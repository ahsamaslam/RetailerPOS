using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;
using System;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Retailer.POS.Web.Services;
public class ApiClient : IApiClient
{

    private readonly HttpClient _http;
    private readonly ILogger<ApiClient> _logger;
    public ApiClient(HttpClient http, ILogger<ApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }
    public async Task<List<ItemDto>> GetItemsAsync()
    {
        var r = await _http.GetAsync("api/items");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<ItemDto>>() ?? new List<ItemDto>();
    }
    public async Task<List<ItemDto>> GetStockItemsAsync(int categoryId, int groupId)
    {
        var r = await _http.GetAsync("api/items/GetStockItemsAsync/" + categoryId.ToString()+"/" + groupId.ToString());
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<ItemDto>>() ?? new List<ItemDto>();
    }
    public async Task<ItemDto?> GetItemAsync(int id)
    {
        var r = await _http.GetAsync($"api/items/{id}");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<ItemDto>();
    }
      public async Task<CompanyDto?> GetCompanyAsync()
    {
        var r = await _http.GetAsync($"api/Companies");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<CompanyDto>();
    }
    public async Task<CompanyDto?> GetUserCompanyAsync()
    {
        var r = await _http.GetAsync($"api/Companies/User");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<CompanyDto>();
    }

    public async Task<(bool Success, string Message)> CreateItemAsync(CreateItemDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/items", dto);
        r.EnsureSuccessStatusCode();
        if (r.IsSuccessStatusCode)
        {
            return (true, "Item Type created successfully");
        }
        else
        {
            var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error";

            return (false, message);
        }

    }
    public async Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/purchases", dto);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<PurchaseMasterDto>() ?? throw new Exception("No purchase returned");
    }
   

    public async Task<string?> LoginAsync(string username, string password)
    {
        var r = await _http.PostAsJsonAsync("api/auth/login", new { username, password });
        if (!r.IsSuccessStatusCode) return null;
        var obj = await r.Content.ReadFromJsonAsync<JsonElement>();
        if (obj.TryGetProperty("token", out var t)) return t.GetString();
        return null;
    }
    public async Task<List<EmployeeDto>> GetEmployeesAsync() =>
        await _http.GetFromJsonAsync<List<EmployeeDto>>("api/employees") ?? new();

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id) =>
        await _http.GetFromJsonAsync<EmployeeDto>($"api/employees/{id}");

    public async Task<bool> CreateEmployeeAsync(EmployeeDto employee)
    {
        var resp = await _http.PostAsJsonAsync("api/employees", employee);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeDto employee)
    {
        var resp = await _http.PutAsJsonAsync($"api/employees/{employee.Id}", employee);
        return resp.IsSuccessStatusCode;
    }

    // Similar for Customer
    public async Task<List<CustomerViewModel>> GetCustomersAsync() =>
        await _http.GetFromJsonAsync<List<CustomerViewModel>>("api/customers") ?? new();

    public async Task<CustomerViewModel?> GetCustomerByIdAsync(int id) =>
        await _http.GetFromJsonAsync<CustomerViewModel>($"api/customers/{id}");

    public async Task<bool> CreateCustomerAsync(CustomerViewModel customer) =>
        (await _http.PostAsJsonAsync("api/customers", customer)).IsSuccessStatusCode;

    public async Task<bool> UpdateCustomerAsync(CustomerViewModel customer) =>
        (await _http.PutAsJsonAsync($"api/customers/{customer.Id}", customer)).IsSuccessStatusCode;

    // Similar for Vendor
    public async Task<List<VendorViewModel>> GetVendorsAsync() =>
        await _http.GetFromJsonAsync<List<VendorViewModel>>("api/vendors") ?? new();

    public async Task<VendorViewModel?> GetVendorByIdAsync(int id) =>
        await _http.GetFromJsonAsync<VendorViewModel>($"api/vendors/{id}");

    public async Task<bool> CreateVendorAsync(VendorViewModel vendor) =>
        (await _http.PostAsJsonAsync("api/vendors", vendor)).IsSuccessStatusCode;

    public async Task<bool> UpdateVendorAsync(VendorViewModel vendor) =>
        (await _http.PutAsJsonAsync($"api/vendors/{vendor.Id}", vendor)).IsSuccessStatusCode;
    // -------- Category --------
    public async Task<List<ItemCategoryViewModel>> GetCategoriesAsync()
        => await _http.GetFromJsonAsync<List<ItemCategoryViewModel>>("api/Categories");

    public async Task<ItemCategoryViewModel?> GetCategoryAsync(int id)
        => await _http.GetFromJsonAsync<ItemCategoryViewModel>($"api/Categories/{id}");

    public async Task<(bool Success, string Message)> CreateCategoryAsync(ItemCategoryViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/Categories", dto);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Category created successfully");
        }
        else
        {
            var content = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error";

            return (false, message);
        }

    }

    public async Task UpdateCategoryAsync(ItemCategoryViewModel dto)
    {

        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("category Id must be set on DTO when updating.");
        var resp = await _http.PutAsJsonAsync($"api/Categories/{dto.Id}", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(int id)
        => await _http.DeleteAsync($"api/Categories/{id}");

    // -------- Group --------
    public async Task<List<ItemGroupViewModel>> GetGroupsAsync()
        => await _http.GetFromJsonAsync<List<ItemGroupViewModel>>("api/groups");

    public async Task<ItemGroupViewModel?> GetGroupAsync(int id)
        => await _http.GetFromJsonAsync<ItemGroupViewModel>($"api/groups/{id}");

    public async Task<(bool Success, string Message)> CreateGroupAsync(ItemGroupViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/groups", dto);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Item Type created successfully");
        }
        else
        {
            var content = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error";

            return (false, message);
        }

    }

    public async Task<bool> UpdateGroupAsync(ItemGroupViewModel dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Id <= 0)
            throw new ArgumentException("Group Id must be set on DTO when updating.");

        var resp = await _http.PutAsJsonAsync($"api/groups/{dto.Id}", dto);

        // Optional: this throws an exception if not successful.
        resp.EnsureSuccessStatusCode();

        // Return whether the request succeeded.
        return resp.IsSuccessStatusCode;
    }

    public async Task DeleteGroupAsync(int id)
        => await _http.DeleteAsync($"api/groups/{id}");

    // -------- SubGroup --------
    public async Task<List<ItemSubGroupViewModel>> GetSubGroupsAsync()
        => await _http.GetFromJsonAsync<List<ItemSubGroupViewModel>>("api/subgroups");

    public async Task<ItemSubGroupViewModel?> GetSubGroupAsync(int id)
        => await _http.GetFromJsonAsync<ItemSubGroupViewModel>($"api/subgroups/{id}");

    public async Task<bool> CreateSubGroupAsync(ItemSubGroupViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/subgroups", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateSubGroupAsync(ItemSubGroupViewModel dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Id <= 0)
            throw new ArgumentException("SubGroup Id must be set on DTO when updating.");

        var resp = await _http.PutAsJsonAsync($"api/subgroups/{dto.Id}", dto);

        // Optional: this throws an exception if not successful.
        resp.EnsureSuccessStatusCode();

        // Return whether the request succeeded.
        return resp.IsSuccessStatusCode;
    }
    public async Task<bool> UpdateItemAsync(ItemDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var resp = await _http.PutAsJsonAsync($"api/items/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task DeleteSubGroupAsync(int id)
        => await _http.DeleteAsync($"api/subgroups/{id}");

    public async Task<List<ItemTypeViewModel>> GetItemTypeAsync() => await _http.GetFromJsonAsync<List<ItemTypeViewModel>>("api/ItemType");

    public async Task<(bool Success, string Message)> CreateItemTypeAsync(ItemTypeViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/ItemType", dto);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Item Type created successfully");
        }
        else
        {
            var content = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error";

            return (false, message);
        }
    }

    public async Task<ItemTypeViewModel?> GetItemTypeAsync(int id) => await _http.GetFromJsonAsync<ItemTypeViewModel>($"api/ItemType/{id}");


    public async Task<bool> UpdateItemTypeAsync(ItemTypeViewModel ItemType)
    {
        var resp = await _http.PutAsJsonAsync($"api/ItemType/{ItemType.Id}", ItemType);
        return resp.IsSuccessStatusCode;
    }

    public async Task<List<PurchaseViewModel>> GetPurchaseDateWiseAsync(DateTime sdate, DateTime edate) => await _http.GetFromJsonAsync<List<PurchaseViewModel>>("api/Purchases/"+ sdate.ToString("yyyy-MM-dd") +"/"+ edate.ToString("yyyy-MM-dd"));
    public async Task<List<PurchaseViewModel>> GetPurchasesAsync() => await _http.GetFromJsonAsync<List<PurchaseViewModel>>("api/Purchases");

    // Branch
    public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<BranchDto>>("api/branch") ?? Enumerable.Empty<BranchDto>();
    }

    public async Task<BranchDto?> GetBranchByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<BranchDto>($"api/branch/{id}");
    }

    public async Task<bool> CreateBranchAsync(BranchDto dto)
    {
        if (dto == null) return false;
        var resp = await _http.PostAsJsonAsync("api/branch", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBranchAsync(BranchDto dto)
    {
        if (dto == null) return false;
        var resp = await _http.PutAsJsonAsync($"api/branch/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBranchAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/branch/{id}");
        return resp.IsSuccessStatusCode;
    }
    // Sales
    public async Task<IEnumerable<SalesMasterDto>> GetSalesAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<SalesMasterDto>>("api/sales") ?? Enumerable.Empty<SalesMasterDto>();
    }

    public async Task<SalesMasterDto?> GetSaleByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<SalesMasterDto>($"api/sales/{id}");
    }

    public async Task<SalesMasterDto?> CreateSaleAsync(SalesMasterDto dto)
    {
        if (dto == null) return null;
        var resp = await _http.PostAsJsonAsync("api/sales", dto);
        return await resp.Content.ReadFromJsonAsync<SalesMasterDto>();
    }

    public async Task<IEnumerable<SalesMasterDto>> GetAllSaleDateWise(DateTime sdate, DateTime edate)
    { 
        var resp = await _http.GetAsync("api/sales/GetAllDateWise/" + sdate.ToString("yyyy-MM-dd") + '/'+edate.ToString("yyyy-MM-dd"));
        return await  resp.Content.ReadFromJsonAsync< IEnumerable<SalesMasterDto>>();
    }

    public async Task<bool> UpdateSaleAsync(SalesMasterDto dto)
    {
        if (dto == null) return false;
        var resp = await _http.PutAsJsonAsync($"api/sales/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSaleAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/sales/{id}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<ScopeDto>> GetAllScopesAsync()
    => await _http.GetFromJsonAsync<IEnumerable<ScopeDto>>("api/scopes") ?? Enumerable.Empty<ScopeDto>();

    public async Task<ScopeDto?> GetScopeByIdAsync(int id)
        => await _http.GetFromJsonAsync<ScopeDto>($"api/scopes/{id}");

    public async Task<bool> CreateScopeAsync(ScopeDto dto)
    {
        var resp = await _http.PostAsJsonAsync("api/scopes", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateScopeAsync(ScopeDto dto)
    {
        var resp = await _http.PutAsJsonAsync($"api/scopes/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteScopeAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/scopes/{id}");
        return resp.IsSuccessStatusCode;
    }
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var result = await _http.GetFromJsonAsync<IEnumerable<RoleDto>>("api/roles");
        return result ?? Enumerable.Empty<RoleDto>();
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        if (id <= 0) return null;
        var resp = await _http.GetAsync($"api/roles/{id}");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<RoleDto>();
    }

    public async Task<bool> CreateRoleAsync(RoleDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var resp = await _http.PostAsJsonAsync("api/roles", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRoleAsync(RoleDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (dto.Id <= 0) throw new ArgumentException("Role Id must be set for update.", nameof(dto));

        var resp = await _http.PutAsJsonAsync($"api/roles/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }
    public async Task<IEnumerable<MenuDto>> GetMenusForCurrentUserAsync()
    {
        // call your POS API endpoint that returns menus for current user:
        var resp = await _http.GetAsync("api/menus/me");
        if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // let caller handle redirect / sign out
            throw new UnauthorizedAccessException();
        }

        resp.EnsureSuccessStatusCode();
        var menus = await resp.Content.ReadFromJsonAsync<IEnumerable<MenuDto>>();
        return menus ?? Enumerable.Empty<MenuDto>();
    }
    public async Task<List<OpeningBalanceViewModel>> GetOpeningBalancesAsync()
    {
        try
        {
            var res = await _http.GetFromJsonAsync<List<OpeningBalanceViewModel>>("api/openingbalances");
            return res ?? new List<OpeningBalanceViewModel>();
        }
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
            var resp = await _http.GetAsync($"api/openingbalances/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<OpeningBalanceViewModel>();
        }
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
            var resp = await _http.PostAsJsonAsync("api/openingbalances", dto);
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadFromJsonAsync<OpeningBalanceViewModel>();
                return new ApiResult<OpeningBalanceViewModel>(true, data);
            }

            if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return new ApiResult<OpeningBalanceViewModel>(false, null, "Opening balance already exists for this Year and Product.");
            }

            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult<OpeningBalanceViewModel>(false, null, err);
        }
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
            var resp = await _http.PutAsJsonAsync($"api/openingbalances/{id}", dto);
            if (resp.IsSuccessStatusCode) return new ApiResult(true);
            if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
                return new ApiResult(false, "Another opening balance exists for this Year and Product.");
            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult(false, err);
        }
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
            var resp = await _http.DeleteAsync($"api/openingbalances/{id}");
            if (resp.IsSuccessStatusCode) return new ApiResult(true);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return new ApiResult(false, "NotFound");
            var err = await resp.Content.ReadAsStringAsync();
            return new ApiResult(false, err);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opening balance {Id}", id);
            return new ApiResult(false, ex.Message);
        }
    }

    public async Task<PurchaseMasterDto?> GetPurchaseByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<PurchaseMasterDto>($"api/Purchases/{id}");
    }

    public async Task<bool> UpdatePurchaseAsync(PurchaseMasterDto dto)
    {
        if (dto == null) return false;
        var resp = await _http.PutAsJsonAsync($"api/Purchases/{dto.Id}", dto);
        return resp.IsSuccessStatusCode;
    }
    // TODO: other methods implemented elsewhere in ApiClient...

    //Menu
    // ------- Menus (add these to your ApiClient class) -------
    public async Task<MenuDto?> CreateMenuAsync(MenuDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var resp = await _http.PostAsJsonAsync("api/menus", dto);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<MenuDto>();
    }

    public async Task<SubMenuDto?> CreateSubMenuAsync(int menuId, SubMenuDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        var resp = await _http.PostAsJsonAsync($"api/menus/{menuId}/submenus", dto);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<SubMenuDto>();
    }

    public async Task<bool> DeleteMenuAsync(int menuId)
    {
        if (menuId <= 0) return false;
        var resp = await _http.DeleteAsync($"api/menus/{menuId}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSubMenuAsync(int menuId, int subMenuId)
    {
        if (menuId <= 0 || subMenuId <= 0) return false;
        var resp = await _http.DeleteAsync($"api/menus/{menuId}/submenus/{subMenuId}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<CompanyDto?> GetCompanybyIdAsync(string guid) => await _http.GetFromJsonAsync<CompanyDto>($"api/Companies/{guid}");

    public async Task<(bool Success, string Message)> UpdateCompanyAsync(CompanyViewModel dto)
    {
        try
        {
            var r = await _http.PutAsJsonAsync($"api/Companies/{dto.Id}", dto);
            r.EnsureSuccessStatusCode();
            if (r.IsSuccessStatusCode)
            {
                return (true, "Company Updated successfully");
            }
            else
            {
                var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                string message = content != null && content.ContainsKey("message")
                    ? content["message"]
                    : "Unknown error";

                return (false, message);
            }
        }
        catch (Exception exx)
        {
            return (false, "Unknown error");
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
      //  return await _http.GetFromJsonAsync<UserDto>("api/User/currentUser");
        var r = await _http.GetAsync("api/User/currentUser");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<UserDto>() ?? new UserDto();
    }

    public async  Task<(bool value, string Message)> ChangePasswordAsync(UserPasswordDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/User/ChangePassword", dto);
        r.EnsureSuccessStatusCode();
        if (r.IsSuccessStatusCode)
        {
            return (true, "Item Type created successfully");
        }
        else
        {
            var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error"; 
            return (false, message);

        }
    }

    public async  Task<(bool value, string Message)> CheckPasswordAsync(UserPasswordDto dto)
    {
        //currentUserPassword
        var r = await _http.PostAsJsonAsync("api/User/currentUserPassword", dto);
        r.EnsureSuccessStatusCode();
        if (r.IsSuccessStatusCode)
        {
            return (true, "Item Type created successfully");
        }
        else
        {
            var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            string message = content != null && content.ContainsKey("message")
                ? content["message"]
                : "Unknown error";

            return (false, message);
        }
    }
}
