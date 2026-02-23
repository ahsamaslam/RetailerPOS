using Microsoft.AspNetCore.Http;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.Web;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using Retailer.Web.Models.Ledger;
using Retailer.Web.ReportDto;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using static QRCoder.PayloadGenerator;

namespace Retailer.POS.Web.Services;
public class ApiClient : IApiClient
{

    private readonly HttpClient _http;
    private readonly IHttpClientFactory _factory;        // Auth Module
    private readonly ILogger<ApiClient> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    public ApiClient(HttpClient http, IHttpClientFactory factory)
    {
        _http = http;
        _factory = factory;
    }
    private HttpClient AuthClient => _factory.CreateClient("AuthApi");
    public async Task<T> GetAsync<T>(string url)
    {
        using var resp = await _http.GetAsync(url);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        // null for empty response will be handled by caller if T allows null
        return await resp.Content.ReadFromJsonAsync<T>(_jsonOptions)
               ?? throw new InvalidOperationException("Response content was empty.");
    }
    public async Task<T> GetAuthAsync<T>(string url)
    {
        using var resp = await AuthClient.GetAsync(url);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        // null for empty response will be handled by caller if T allows null
        return await resp.Content.ReadFromJsonAsync<T>(_jsonOptions)
               ?? throw new InvalidOperationException("Response content was empty.");
    }

    

    // Generic POST that returns typed response
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body)
    {
        using var resp = await _http.PostAsJsonAsync(url, body, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body)
    {
        using var resp = await _http.PutAsJsonAsync(url, body, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public async Task DeleteAsync(string url)
    {
        using var resp = await _http.DeleteAsync(url);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
    }
    /// <summary>
    /// Helper to parse error message (if server returns JSON { message: "..." }).
    /// Falls back to raw response body string.
    /// </summary>
    private async Task<string> ReadErrorMessageAsync(HttpResponseMessage resp)
    {
        try
        {
            var dict = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonOptions);
            if (dict != null && dict.TryGetValue("message", out var m)) return m;
        }
        catch { /* ignore JSON parse issues */ }

        try
        {
            return await resp.Content.ReadAsStringAsync();
        }
        catch { return "Unknown error"; }
    }
    public async Task<List<ItemDto>> GetItemsAsync()
    {
        return await GetAsync<List<ItemDto>>("api/items");
    }
    public async Task<List<CitiesDto>> GetCitiesAsync()
    {
        return await GetAsync<List<CitiesDto>>("api/Cities");
    }
    public async Task<List<ProvienceDto>> GetProvienceAsync()
    {
        return await GetAsync<List<ProvienceDto>>("api/Provience");
    }
    public async Task<List<ItemDto>> GetStockItemsAsync(int categoryId, int groupId)
    {
        return await GetAsync<List<ItemDto>>("api/items/GetStockItemsAsync/" + categoryId.ToString() + "/" + groupId.ToString());
    }
    
    // Items
    public async Task<ItemDto?> GetItemAsync(int id) => await GetAsync<ItemDto>($"api/items/{id}");

    public async Task<(bool Success, string Message)> CreateItemAsync(CreateItemDto dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/items", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode) return (true, "Item Type created successfully");

        var message = await ReadErrorMessageAsync(resp);
        return (false, message);
    }

    public async Task<bool> UpdateItemAsync(ItemDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        using var resp = await _http.PutAsJsonAsync($"api/items/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/items/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Companies
     public async Task<IEnumerable<CompanyDto>> GetCompanyAsync() => await GetAuthAsync<IEnumerable<CompanyDto>>("api/Companies");
 
     public async Task<CompanyDto?> GetUserCompanyAsync() => await GetAuthAsync<CompanyDto>("api/Companies/User");
 
    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id)
    {
        using var resp = await AuthClient.GetAsync($"api/Companies/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
    }

    public async Task<(bool Success, string Message, CompanyDto? Company)> CreateCompanyAsync(CompanyDto dto)
    {
        if (dto == null) return (false, "Company information is required.", null);

        using var resp = await AuthClient.PostAsJsonAsync("api/Companies", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode)
        {
            var created = await resp.Content.ReadFromJsonAsync<CompanyDto>(_jsonOptions);
            // after creating company in Auth module, create default branch in Retailer API
            try
            {
                if (created != null)
                {
                    var branch = new BranchDto { Name = "default" };
                    // best-effort: create default branch in Retailer API using tenant header
                    _ = await CreateBranchAsync(branch, created.Id);
                }
            }
            catch
            {
                // ignore errors - non-fatal
            }

            return (true, "Company created successfully", created);
        }

        var message = await ReadErrorMessageAsync(resp);
        return (false, message, null);
    }

    public async Task<(bool Success, string Message)> UpdateCompanyAsync(Guid id, CompanyDto dto)
    {
        if (dto == null) return (false, "Company information is required.");

        using var resp = await AuthClient.PutAsJsonAsync($"api/Companies/{id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode) return (true, "Company updated successfully");

        var message = await ReadErrorMessageAsync(resp);
        return (false, message);
    }

    // Authentication
    public async Task<string?> LoginAsync(string username, string password)
    {
        using var r = await _http.PostAsJsonAsync("api/auth/login", new { username, password }, _jsonOptions);
        if (r.StatusCode == HttpStatusCode.Unauthorized) return null;
        if (!r.IsSuccessStatusCode) return null;

        var obj = await r.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
        if (obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty("token", out var t))
            return t.GetString();

        return null;
    }

    // Employees
    public async Task<List<EmployeeDto>> GetEmployeesAsync() =>
        await GetAsync<List<EmployeeDto>>("api/employee") ?? new List<EmployeeDto>();

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id) => await GetAsync<EmployeeDto>($"api/employee/{id}");

    public async Task<bool> CreateEmployeeAsync(EmployeeDto employee)
    {
        using var resp = await _http.PostAsJsonAsync("api/employee", employee, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeDto employee)
    {
        using var resp = await _http.PutAsJsonAsync($"api/employee/{employee.Id}", employee, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
    // Payment Method 
    public async Task<List<PaymentMethodDto>> GetPaymentMethodAsync() =>
       await GetAsync<List<PaymentMethodDto>>("api/paymentMethod") ?? new List<PaymentMethodDto>();
    // Customers
    public async Task<List<CustomerViewModel>> GetCustomersAsync() =>
        await GetAsync<List<CustomerViewModel>>("api/customers") ?? new List<CustomerViewModel>();
    // Banks
    public async Task<List<BanksViewModel>> GetBanksAsync() =>
        await GetAsync<List<BanksViewModel>>("api/banks") ?? new List<BanksViewModel>();
    public async Task<CustomerViewModel?> GetCustomerByIdAsync(int id) => await GetAsync<CustomerViewModel>($"api/customers/{id}");
    public async Task<double> GetCustomersBalanceAsync(DateTime edate,int id) => await GetAsync<double>($"api/CustomerLedger/{edate.ToString("yyyy-MM-dd")}/{id}");
    public async Task<double> GetVendorBalanceAsync(DateTime edate,int id) => await GetAsync<double>($"api/VendorLedger/{edate.ToString("yyyy-MM-dd")}/{id}");
    public async Task<BanksViewModel?> GetBankByIdAsync(int id) => await GetAsync<BanksViewModel>($"api/banks/{id}");

    public async Task<bool> CreateCustomerAsync(CustomerViewModel customer)
    {
        using var resp = await _http.PostAsJsonAsync("api/customers", customer, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
    public async Task<bool> CreateCustomerPaymentAsync(CustomerPaymentViewModel customer)
    {
        using var resp = await _http.PostAsJsonAsync("api/CustomerPayment", customer, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }  
    public async Task<bool> CreateVendorPaymentAsync(VendorPaymentViewModel vendor)
    {
        using var resp = await _http.PostAsJsonAsync("api/vendorPayment", vendor, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
    public async Task<bool> CreateBankAsync(BanksViewModel customer)
    {
        using var resp = await _http.PostAsJsonAsync("api/Banks", customer, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCustomerAsync(CustomerViewModel customer)
    {
        using var resp = await _http.PutAsJsonAsync($"api/customers/{customer.Id}", customer, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
     public async Task<bool> UpdateBankAsync(BanksViewModel bank)
    {
        using var resp = await _http.PutAsJsonAsync($"api/banks/{bank.Id}", bank, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Vendors
    public async Task<List<VendorViewModel>> GetVendorsAsync() =>
        await GetAsync<List<VendorViewModel>>("api/vendors") ?? new List<VendorViewModel>();

    public async Task<VendorViewModel?> GetVendorByIdAsync(int id) => await GetAsync<VendorViewModel>($"api/vendors/{id}");

    public async Task<bool> CreateVendorAsync(VendorViewModel vendor)
    {
        using var resp = await _http.PostAsJsonAsync("api/vendors", vendor, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateVendorAsync(VendorViewModel vendor)
    {
        using var resp = await _http.PutAsJsonAsync($"api/vendors/{vendor.Id}", vendor, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Categories
    public async Task<List<ItemCategoryViewModel>> GetCategoriesAsync() =>
        await GetAsync<List<ItemCategoryViewModel>>("api/Categories") ?? new List<ItemCategoryViewModel>();

    public async Task<ItemCategoryViewModel?> GetCategoryAsync(int id) => await GetAsync<ItemCategoryViewModel>($"api/Categories/{id}");

    public async Task<(bool Success, string Message)> CreateCategoryAsync(ItemCategoryViewModel dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/Categories", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode) return (true, "Category created successfully");

        var message = await ReadErrorMessageAsync(resp);
        return (false, message);
    }

    public async Task UpdateCategoryAsync(ItemCategoryViewModel dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("category Id must be set on DTO when updating.");
        using var resp = await _http.PutAsJsonAsync($"api/Categories/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/Categories/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
    }

    // Groups
    public async Task<List<ItemGroupViewModel>> GetGroupsAsync() =>
        await GetAsync<List<ItemGroupViewModel>>("api/groups") ?? new List<ItemGroupViewModel>();

    public async Task<ItemGroupViewModel?> GetGroupAsync(int id) => await GetAsync<ItemGroupViewModel>($"api/groups/{id}");

    public async Task<(bool Success, string Message)> CreateGroupAsync(ItemGroupViewModel dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/groups", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode) return (true, "Item Type created successfully");

        var message = await ReadErrorMessageAsync(resp);
        return (false, message);
    }

    public async Task<bool> UpdateGroupAsync(ItemGroupViewModel dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("Group Id must be set on DTO when updating.");

        using var resp = await _http.PutAsJsonAsync($"api/groups/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        return resp.IsSuccessStatusCode;
    }

    public async Task DeleteGroupAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/groups/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
    }

    // SubGroups
    public async Task<List<ItemSubGroupViewModel>> GetSubGroupsAsync() =>
        await GetAsync<List<ItemSubGroupViewModel>>("api/subgroups") ?? new List<ItemSubGroupViewModel>();

    public async Task<ItemSubGroupViewModel?> GetSubGroupAsync(int id) => await GetAsync<ItemSubGroupViewModel>($"api/subgroups/{id}");

    public async Task<bool> CreateSubGroupAsync(ItemSubGroupViewModel dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/subgroups", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSubGroupAsync(ItemSubGroupViewModel dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("SubGroup Id must be set on DTO when updating.");

        using var resp = await _http.PutAsJsonAsync($"api/subgroups/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        resp.EnsureSuccessStatusCode();
        return resp.IsSuccessStatusCode;
    }

    public async Task DeleteSubGroupAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/subgroups/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
    }

    // Item types
    public async Task<List<ItemTypeViewModel>> GetItemTypeAsync() =>
        await GetAsync<List<ItemTypeViewModel>>("api/ItemType") ?? new List<ItemTypeViewModel>();

    public async Task<(bool Success, string Message)> CreateItemTypeAsync(ItemTypeViewModel dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/ItemType", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

        if (resp.IsSuccessStatusCode) return (true, "Item Type created successfully");
        var message = await ReadErrorMessageAsync(resp);
        return (false, message);
    }

    public async Task<ItemTypeViewModel?> GetItemTypeAsync(int id) => await GetAsync<ItemTypeViewModel>($"api/ItemType/{id}");

    public async Task<bool> UpdateItemTypeAsync(ItemTypeViewModel ItemType)
    {
        using var resp = await _http.PutAsJsonAsync($"api/ItemType/{ItemType.Id}", ItemType, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Purchases
    public async Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/purchases", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<PurchaseMasterDto>(_jsonOptions)
               ?? throw new Exception("No purchase returned");
    }
    public async Task<PurchaseReturnMasterDto> CreatePurchaseReturnAsync(CreatePurchaseReturnDto dto)
    {
        try
        {
            using var resp = await _http.PostAsJsonAsync("api/purchasereturn", dto, _jsonOptions);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<PurchaseReturnMasterDto>(_jsonOptions)
                   ?? throw new Exception("No purchase returned");
        }
        catch (Exception exx)
        {
              throw new Exception("No purchase returned");
        }
    }

    public async Task<List<PurchaseReturnViewModel>> GetPurchaseReturnDateWiseAsync(DateTime sdate, DateTime edate) =>
        await GetAsync<List<PurchaseReturnViewModel>>($"api/PurchaseReturn/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<PurchaseReturnViewModel>();

    public async Task<List<PurchaseReturnViewModel>> GetPurchaseReturnVendorWiseAsync(int vendorID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<PurchaseReturnViewModel>>($"api/PurchaseReturn/VendorWise/{vendorID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<PurchaseReturnViewModel>();

    public async Task<List<ItemPurchaseReport>> GetPurchaseReturnItemWiseAsync(int itemID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<ItemPurchaseReport>>($"api/PurchaseReturn/ItemWise/{itemID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<ItemPurchaseReport>();

    public async Task<List<PurchaseViewModel>> GetPurchaseDateWiseAsync(DateTime sdate, DateTime edate) =>
        await GetAsync<List<PurchaseViewModel>>($"api/Purchases/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<PurchaseViewModel>();

    public async Task<List<PurchaseViewModel>> GetPurchaseVendorWiseAsync(int vendorID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<PurchaseViewModel>>($"api/Purchases/VendorWise/{vendorID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<PurchaseViewModel>();

    public async Task<List<ItemPurchaseReport>> GetPurchaseItemWiseAsync(int itemID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<ItemPurchaseReport>>($"api/Purchases/ItemWise/{itemID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<ItemPurchaseReport>();


    public async Task<List<SalesReturnViewModel>> GetSalesReturnDateWiseAsync(DateTime sdate, DateTime edate) =>
        await GetAsync<List<SalesReturnViewModel>>($"api/SalesReturn/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<SalesReturnViewModel>();

    public async Task<List<SalesReturnViewModel>> GetSalesReturnCustomerWiseAsync(int customerID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<SalesReturnViewModel>>($"api/SalesReturn/CustomerWise/{customerID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<SalesReturnViewModel>();

    public async Task<List<ItemSalesReturnReport>> GetSalesReturnItemWiseAsync(int itemID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<ItemSalesReturnReport>>($"api/SalesReturn/ItemWise/{itemID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<ItemSalesReturnReport>();

    public async Task<List<SalesViewModel>> GetSalesDateWiseAsync(DateTime sdate, DateTime edate) =>
        await GetAsync<List<SalesViewModel>>($"api/Sales/GetAllDateWise/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<SalesViewModel>();

    public async Task<List<SalesViewModel>> GetSalesCustomerWiseAsync(int customerID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<SalesViewModel>>($"api/Sales/CustomerWise/{customerID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<SalesViewModel>();

    public async Task<List<ItemSalesReport>> GetSalesItemWiseAsync(int itemID, DateTime sdate, DateTime edate) =>
        await GetAsync<List<ItemSalesReport>>($"api/Sales/ItemWise/{itemID}/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}") ?? new List<ItemSalesReport>();

    public async Task<List<PurchaseViewModel>> GetPurchasesAsync() =>
        await GetAsync<List<PurchaseViewModel>>("api/Purchases") ?? new List<PurchaseViewModel>();

    public async Task<PurchaseMasterDto?> GetPurchaseByIdAsync(int id) {
		try
		{

			var response = await _http.GetAsync($"api/Purchases/{id}");
			string va = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
				return null;
        
			return JsonSerializer.Deserialize<PurchaseMasterDto>(
		  va,
		  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
	  );
		}
		catch(Exception ex)
        {
			return null;
		}   
    
    }
    public async Task<PurchaseReturnMasterDto?> GetPurchaseReturnByIdAsync(int id) {
		try
		{

			var response = await _http.GetAsync($"api/Purchasereturn/{id}");
			string va = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
				return null;
        
			return JsonSerializer.Deserialize<PurchaseReturnMasterDto>(
		  va,
		  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
	  );
		}
		catch(Exception ex)
        {
			return null;
		}   
    
    }

    public async Task<bool> UpdatePurchaseReturnAsync(PurchaseReturnMasterDto dto)
    {
        if (dto == null) return false;

        string json =  JsonSerializer.Serialize(dto);   
        using var resp = await _http.PutAsJsonAsync($"api/Purchasereturn/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePurchaseAsync(PurchaseMasterDto dto)
    {
        if (dto == null) return false;

        string json =  JsonSerializer.Serialize(dto);   
        using var resp = await _http.PutAsJsonAsync($"api/Purchases/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Branch
    public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync() =>
        await GetAsync<IEnumerable<BranchDto>>("api/branch") ?? Array.Empty<BranchDto>();

    public async Task<BranchDto?> GetBranchByIdAsync(int id) => await GetAsync<BranchDto>($"api/branch/{id}");

    public async Task<(bool Success, string Message)> CreateBranchAsync(BranchDto dto, Guid? companyId = null)
    {
        if (dto == null) return (false, "Branch payload required");
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "api/branch") { Content = JsonContent.Create(dto, options: _jsonOptions) };
        if (companyId.HasValue)
            req.Headers.Add("X-Company-Id", companyId.Value.ToString());

        using var resp = await _http.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return (resp.IsSuccessStatusCode, resp.IsSuccessStatusCode ? string.Empty : await ReadErrorMessageAsync(resp));
    }

    public async Task<bool> UpdateBranchAsync(BranchDto dto)
    {
        if (dto == null) return false;
        using var resp = await _http.PutAsJsonAsync($"api/branch/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBranchAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/branch/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Sales
    public async Task<IEnumerable<SalesMasterDto>> GetSalesAsync() =>
        await GetAsync<IEnumerable<SalesMasterDto>>("api/sales") ?? Array.Empty<SalesMasterDto>();

    public async Task<SalesMasterDto?> GetSaleByIdAsync(int id) => await GetAsync<SalesMasterDto>($"api/sales/{id}");
    public async Task<SaleInvoiceSettingDto?> GetSalePrintSetting(int id) => await GetAsync<SaleInvoiceSettingDto>($"api/SalesSetting/{id}"); 
    public async Task<List<SaleInvoiceSettingDto>?> GetSalePrintSettingList() => await GetAsync<List<SaleInvoiceSettingDto>>($"api/SalesSetting");
    public async Task<SalesMasterReturnDto?> GetSaleReturnByIdAsync(int id) => await GetAsync<SalesMasterReturnDto>($"api/salesreturn/{id}");
    public async Task<CustomerPaymentDto?> GetcustomerpaymentByIdAsync(int id) => await GetAsync<CustomerPaymentDto>($"api/customerpayment/{id}");

    public async Task<SalesMasterDto?> CreateSaleAsync(SalesMasterDto dto)
    {
        if (dto == null) return null;

        string json = JsonSerializer.Serialize(dto);
        using var resp = await _http.PostAsJsonAsync("api/sales", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<SalesMasterDto>(_jsonOptions);
    }
    public async Task<SalesMasterReturnDto?> CreateSaleAsync(SalesMasterReturnDto dto)
    {
        if (dto == null) return null;
        using var resp = await _http.PostAsJsonAsync("api/salesreturn", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<SalesMasterReturnDto>(_jsonOptions);
    }
    public async Task<IEnumerable<CustomerPaymentDto>> GetAllCustomerPaymentDateWise(DateTime sdate, DateTime edate)
    {
        using var resp = await _http.GetAsync($"api/customerpayment/GetAllDateWise/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<CustomerPaymentDto>>(_jsonOptions) ?? Array.Empty<CustomerPaymentDto>();
    }
    public async Task<IEnumerable<CustomerLedgerDto>> GetCustomerLedgerAsync(DateTime sdate, DateTime edate, int customerCode)
    {
        using var resp = await _http.GetAsync($"api/customerledger/Ledger/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}/{ customerCode}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<CustomerLedgerDto>>(_jsonOptions) ?? Array.Empty<CustomerLedgerDto>();
    }
    public async Task<IEnumerable<ItemLedgerDto>> GetItemLedgerAsync(DateTime sdate, DateTime edate, int customerCode)
    {
        using var resp = await _http.GetAsync($"api/itemledger/Ledger/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}/{ customerCode}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<ItemLedgerDto>>(_jsonOptions) ?? Array.Empty<ItemLedgerDto>();
    }
    public async Task<IEnumerable<VendorLedgerDto>> GetVendorLedgerAsync(DateTime sdate, DateTime edate, int vendorCode)
    {
        using var resp = await _http.GetAsync($"api/vendorledger/Ledger/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}/{vendorCode}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<VendorLedgerDto>>(_jsonOptions) ?? Array.Empty<VendorLedgerDto>();
    }
    public async Task<IEnumerable<VendorPaymentDto>> GetAllVendorPaymentDateWise(DateTime sdate, DateTime edate)
    {
        try
        
        {
            
            using var resp = await _http.GetAsync($"api/vendorpayment/GetAllDateWise/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            string json = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<IEnumerable<VendorPaymentDto>>(_jsonOptions) ?? Array.Empty<VendorPaymentDto>();
        }
        catch (Exception exx)
        {
          return  Array.Empty<VendorPaymentDto>();
        }
    }
    public async Task<IEnumerable<SalesMasterDto>> GetAllSaleDateWise(DateTime sdate, DateTime edate)
    {
        using var resp = await _http.GetAsync($"api/sales/GetAllDateWise/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<SalesMasterDto>>(_jsonOptions) ?? Array.Empty<SalesMasterDto>();
    }
    public async Task<IEnumerable<SalesMasterReturnDto>> GetAllSaleReturnDateWise(DateTime sdate, DateTime edate)
    {
        using var resp = await _http.GetAsync($"api/salesreturn/GetAllDateWise/{sdate:yyyy-MM-dd}/{edate:yyyy-MM-dd}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<SalesMasterReturnDto>>(_jsonOptions) ?? Array.Empty<SalesMasterReturnDto>();
    }
    public async Task<bool> UpdateSaleAsync(SalesMasterDto dto)
    {
        if (dto == null) return false;
        using var resp = await _http.PutAsJsonAsync($"api/sales/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
    public async Task<bool> UpdateSaleReturnAsync(SalesMasterReturnDto dto)
    {
        if (dto == null) return false;
        using var resp = await _http.PutAsJsonAsync($"api/salesreturn/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
       public async Task<bool> UpdateCustomerPaymentAsync(CustomerPaymentDto dto)
    {
        if (dto == null) return false;
        using var resp = await _http.PutAsJsonAsync($"api/customerpayment/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }
         public async Task<bool> UpdateVendorPaymentAsync(VendorPaymentDto dto)
    {
        if (dto == null) return false;
        using var resp = await _http.PutAsJsonAsync($"api/vendorpayment/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSaleAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/sales/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSaleReturnAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/salesreturn/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Scopes
    public async Task<IEnumerable<ScopeDto>> GetAllScopesAsync() =>
        await GetAsync<IEnumerable<ScopeDto>>("api/scopes") ?? Array.Empty<ScopeDto>();

    public async Task<ScopeDto?> GetScopeByIdAsync(int id) => await GetAsync<ScopeDto>($"api/scopes/{id}");

    public async Task<bool> CreateScopeAsync(ScopeDto dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/scopes", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateScopeAsync(ScopeDto dto)
    {
        using var resp = await _http.PutAsJsonAsync($"api/scopes/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteScopeAsync(int id)
    {
        using var resp = await _http.DeleteAsync($"api/scopes/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Roles
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync() =>
        await GetAsync<IEnumerable<RoleDto>>("api/roles") ?? Array.Empty<RoleDto>();

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        if (id <= 0) return null;
        using var resp = await _http.GetAsync($"api/roles/{id}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<RoleDto>(_jsonOptions);
    }

    public async Task<bool> CreateRoleAsync(RoleDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        using var resp = await _http.PostAsJsonAsync("api/roles", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRoleAsync(RoleDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("Role Id must be set for update.", nameof(dto));

        using var resp = await _http.PutAsJsonAsync($"api/roles/{dto.Id}", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    }

    // Menus
    public async Task<IEnumerable<MenuDto>> GetMenusForCurrentUserAsync()
    {
        using var resp = await _http.GetAsync("api/menus/me");
        var body = await resp.Content.ReadAsStringAsync();
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IEnumerable<MenuDto>>(_jsonOptions) ?? Array.Empty<MenuDto>();
    }

    public async Task<MenuDto?> CreateMenuAsync(MenuDto dto)
    {
        using var resp = await _http.PostAsJsonAsync("api/menus", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<MenuDto>(_jsonOptions);
    } 
    public async Task<SubMenuDto?> CreateSubMenuAsync(int menuId, SubMenuDto dto)
    {
        using var resp = await _http.PostAsJsonAsync($"api/menus/{menuId}/submenus", dto, _jsonOptions);
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<SubMenuDto>(_jsonOptions);
    }

    public async Task<bool> DeleteMenuAsync(int menuId)
    {
        if (menuId <= 0) return false;
        using var resp = await _http.DeleteAsync($"api/menus/{menuId}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    } 
    public async Task<bool> DeleteSubMenuAsync(int menuId, int subMenuId)
    {
        if (menuId <= 0 || subMenuId <= 0) return false;
        using var resp = await _http.DeleteAsync($"api/menus/{menuId}/submenus/{subMenuId}");
        if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
        return resp.IsSuccessStatusCode;
    } 
    // Opening balances
    public async Task<List<OpeningBalanceViewModel>> GetOpeningBalancesAsync()
    {
        try
        {
            return await GetAsync<List<OpeningBalanceViewModel>>("api/openingbalances") ?? new List<OpeningBalanceViewModel>();
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opening balances");
            return new List<OpeningBalanceViewModel>();
        }
    } 
    public async Task<OpeningBalanceViewModel?> GetOpeningBalanceAsync(int id)
    {
        try
        {
            using var resp = await _http.GetAsync($"api/openingbalances/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<OpeningBalanceViewModel>(_jsonOptions);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting opening balance {Id}", id);
            return null;
        }
    } 
    public async Task<ApiResult<OpeningBalanceViewModel>> CreateOpeningBalanceAsync(CreateOpeningBalanceDto dto)
    {
        try
        {
            using var resp = await _http.PostAsJsonAsync("api/openingbalances", dto, _jsonOptions);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<OpeningBalanceViewModel>(_jsonOptions);
                return new ApiResult<OpeningBalanceViewModel>(true, data);
            }

            if (resp.StatusCode == HttpStatusCode.Conflict)
            {
                return new ApiResult<OpeningBalanceViewModel>(false, null, "Opening balance already exists for this Year and Product.");
            }

            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult<OpeningBalanceViewModel>(false, null, err);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating opening balance");
            return new ApiResult<OpeningBalanceViewModel>(false, null, ex.Message);
        }
    } 
    public async Task<ApiResult> UpdateOpeningBalanceAsync(int id, UpdateOpeningBalanceDto dto)
    {
        try
        {
            using var resp = await _http.PutAsJsonAsync($"api/openingbalances/{id}", dto, _jsonOptions);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return new ApiResult(true);
            if (resp.StatusCode == HttpStatusCode.Conflict)
                return new ApiResult(false, "Another opening balance exists for this Year and Product.");
            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult(false, err);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opening balance {Id}", id);
            return new ApiResult(false, ex.Message);
        }
    } 
    public async Task<ApiResult> DeleteOpeningBalanceAsync(int id)
    {
        try
        {
            using var resp = await _http.DeleteAsync($"api/openingbalances/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return new ApiResult(true);
            if (resp.StatusCode == HttpStatusCode.NotFound) return new ApiResult(false, "NotFound");
            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult(false, err);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return new ApiResult(false, ex.Message);
        }
    }

    public async Task<CustomerPaymentViewModel?> GetCustomerpaymentByIdAsync(int id) => await GetAsync<CustomerPaymentViewModel>($"api/customerpayment/{id}");
    public async Task<VendorPaymentViewModel?> GetVendorpaymentByIdAsync(int id) => await GetAsync<VendorPaymentViewModel>($"api/vendorpayment/{id}");

    public async Task<bool> DeleteCustomerPaymentAsync(int id)
    {
        try
        {
            using var resp = await _http.DeleteAsync($"api/customerpayment/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return (true);
            if (resp.StatusCode == HttpStatusCode.NotFound) return (false);
            var err = await resp.Content.ReadAsStringAsync();
            return    (false);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return (false);
        }

    }
    
    public async Task<bool> DeleteVendorPaymentAsync(int id)
    {
        try
        {
            using var resp = await _http.DeleteAsync($"api/vendorpayment/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return (true);
            if (resp.StatusCode == HttpStatusCode.NotFound) return (false);
            var err = await resp.Content.ReadAsStringAsync();
            return    (false);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return (false);
        }

    }
    
    public async Task<bool> DeletePurchaseAsync(int id)
    {
        try
        {
            using var resp = await _http.DeleteAsync($"api/purchases/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return (true);
            if (resp.StatusCode == HttpStatusCode.NotFound) return (false);
            var err = await resp.Content.ReadAsStringAsync();
            return    (false);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return (false);
        }

    }
    
    public async Task<bool> DeletePurchaseReturnAsync(int id)
    {
        try
        {
            using var resp = await _http.DeleteAsync($"api/purchasereturn/{id}");
            if (resp.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();

            if (resp.IsSuccessStatusCode) return (true);
            if (resp.StatusCode == HttpStatusCode.NotFound) return (false);
            var err = await resp.Content.ReadAsStringAsync();
            return    (false);
        }
        catch (ApiUnauthorizedException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return (false);
        }

    }

    // add alongside the other item helpers
    public async Task<List<ItemDto>> SearchItemsAsync(int catID  ,string? term, int take = 20)
    {
        var size = Math.Clamp(take, 1, 50);

        var encodedTerm = string.IsNullOrWhiteSpace(term)
            ? string.Empty
            : $"&term={Uri.EscapeDataString(term)}";

        return await GetAsync<List<ItemDto>>(
            $"api/items/search?catd={catID}&take={size}{encodedTerm}"
        );
    }


    //public async Task<PurchaseMasterDto?> GetPurchaseByIdAsync(int id) => await GetAsync<PurchaseMasterDto>($"api/Purchases/{id}");

    // Password & user-related

    public async Task<(bool Success, UploadDataResultDto? Result, string Message)> UploadDataAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null)
        {
            return (false, null, "No file provided.");
        }

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "text/csv");
        content.Add(streamContent, "file", file.FileName);

        using var resp = await _http.PostAsync("api/upload-data", content, cancellationToken);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        if (!resp.IsSuccessStatusCode)
        {
            var message = await ReadErrorMessageAsync(resp);
            return (false, null, message);
        }

        var result = await resp.Content.ReadFromJsonAsync<UploadDataResultDto>(_jsonOptions);
        if (result == null)
        {
            return (false, null, "Upload completed but no summary was returned.");
        }

        return (true, result, "Upload completed successfully.");
    }
    public async Task<(bool Success, UploadDataResultDto? Result, string Message)> UploadStockAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null)
        {
            return (false, null, "No file provided.");
        }

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "text/csv");
        content.Add(streamContent, "file", file.FileName);

        using var resp = await _http.PostAsync("api/upload-stock", content, cancellationToken);
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException();

        if (!resp.IsSuccessStatusCode)
        {
            var message = await ReadErrorMessageAsync(resp);
            return (false, null, message);
        }

        var result = await resp.Content.ReadFromJsonAsync<UploadDataResultDto>(_jsonOptions);
        if (result == null)
        {
            return (false, null, "Upload completed but no summary was returned.");
        }

        return (true, result, "Upload completed successfully.");
    }

    public async Task<byte[]> ItemCsvExport()
    {
        var url = $"api/items/export-csv";
        return await _http.GetByteArrayAsync(url);
    }
    #region Purchase Report
    public async Task<byte[]> ExportPurchaseDateWiseAsync(string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReportExport/date-wise?export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportPurchaseVendorWiseAsync(int vendorID, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReportExport/vendor-wise?vendorid={vendorID}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportPurchaseItemWiseAsync(int ItemID, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReportExport/item-wise?itemid={ItemID}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportPurchaseReturnDateWiseAsync(string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReturnReportExport/date-wise?export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportPurchaseReturnVendorWiseAsync(int vendorId, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReturnReportExport/vendor-wise?vendorId={vendorId}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportPurchaseReturnItemWiseAsync(int itemId, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/PurchaseReturnReportExport/item-wise?itemId={itemId}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    #endregion


    #region Sales Report
    public async Task<byte[]> ExportSalesDateWiseAsync(string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReportExport/date-wise?export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportSalesCustomerWiseAsync(int customerId, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReportExport/customer-wise?customerid={customerId}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportSalesItemWiseAsync(int ItemID, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReportExport/item-wise?itemid={ItemID}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportSalesReturnDateWiseAsync(string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReturnReportExport/date-wise?export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportSalesReturnCustomerWiseAsync(int customerId, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReturnReportExport/customer-wise?customerId={customerId}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    public async Task<byte[]> ExportSalesReturnItemWiseAsync(int itemId, string export, DateTime sdate, DateTime edate)
    {
        var url = $"api/SalesReturnReportExport/item-wise?itemId={itemId}&export={export}&sdate={sdate:yyyy-MM-dd}&edate={edate:yyyy-MM-dd}";
        return await _http.GetByteArrayAsync(url);
    }
    #endregion
}
