using Retailer.POS.Web.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using Retailer.Web.Models;
using Retailer.POS.Web.DTOs;
using static System.Net.WebRequestMethods;

namespace Retailer.POS.Web.Services;
public class ApiClient : IApiClient
{

    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        var r = await _http.GetAsync("api/items");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<ItemDto>>() ?? new List<ItemDto>();
    }

    public async Task<ItemDto?> GetItemAsync(int id)
    {
        var r = await _http.GetAsync($"api/items/{id}");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<ItemDto>();
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemDto dto)
    {
        var r = await _http.PostAsJsonAsync("api/items", dto);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<ItemDto>() ?? throw new Exception("No item returned");
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

    public async Task<List<Branch>> GetBranchesAsync()
    {
        var r = await _http.GetAsync("api/branches");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<List<Branch>>() ?? new List<Branch>();
    }

    // Example Employee API calls
    public async Task<List<EmployeeViewModel>> GetEmployeesAsync() =>
        await _http.GetFromJsonAsync<List<EmployeeViewModel>>("api/employees") ?? new();

    public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id) =>
        await _http.GetFromJsonAsync<EmployeeViewModel>($"api/employees/{id}");

    public async Task<bool> CreateEmployeeAsync(EmployeeViewModel employee)
    {
        var resp = await _http.PostAsJsonAsync("api/employees", employee);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeViewModel employee)
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

    // Sales
    public async Task<bool> CreateSalesAsync(SalesViewModel sale) =>
        (await _http.PostAsJsonAsync("api/sales", sale)).IsSuccessStatusCode;

    public async Task<SalesViewModel?> GetSaleByIdAsync(int id) =>
        await _http.GetFromJsonAsync<SalesViewModel>($"api/sales/{id}");
    public async Task UpdateSaleAsync(SalesViewModel sale)
    {
        var response = await _http.PutAsJsonAsync($"api/Sales/{sale.Id}", sale);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSaleAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Sales/{id}");
        response.EnsureSuccessStatusCode();
    }
    public async Task<List<SalesViewModel>> GetSalesAsync()
    {
        return await _http.GetFromJsonAsync<List<SalesViewModel>>("api/Sales");
    }
    // -------- Category --------
    public async Task<List<ItemCategoryViewModel>> GetCategoriesAsync()
        => await _http.GetFromJsonAsync<List<ItemCategoryViewModel>>("api/Categories");

    public async Task<ItemCategoryViewModel?> GetCategoryAsync(int id)
        => await _http.GetFromJsonAsync<ItemCategoryViewModel>($"api/Categories/{id}");

    public async Task<bool> CreateCategoryAsync(ItemCategoryViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/Categories", dto);
        return resp.IsSuccessStatusCode;
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

    public async Task<bool> CreateGroupAsync(ItemGroupViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/groups", dto);
        return resp.IsSuccessStatusCode;
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

    public async Task<bool> CreateItemTypeAsync(ItemTypeViewModel dto)
    {
        var resp = await _http.PostAsJsonAsync("api/ItemType", dto);
        return resp.IsSuccessStatusCode;
    }

    public async Task<ItemTypeViewModel?> GetItemTypeAsync(int id)  => await _http.GetFromJsonAsync<ItemTypeViewModel>($"api/ItemType/{id}");
    

    public async Task<bool> UpdateItemTypeAsync(ItemTypeViewModel ItemType)
    {
        var resp = await _http.PutAsJsonAsync($"api/ItemType/{ItemType.Id}", ItemType);
        return resp.IsSuccessStatusCode;
    }

    public async Task<List<PurchaseViewModel>> GetPurchasesAsync() => await _http.GetFromJsonAsync<List<PurchaseViewModel>>("api/Purchases");
    
}
