using System.Net.Http.Json;
using Retailer.POS.Web.Models;
using Retailer.POS.Api.DTOs;
using System.Text.Json;
using System.Net.Http.Headers;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Services;
public class ApiClient : IApiClient
{

    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<ItemDto>> GetItemsAsync()
    {
        var r = await _http.GetAsync("api/items");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>() ?? Enumerable.Empty<ItemDto>();
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

    public async Task<IEnumerable<Branch>> GetBranchesAsync()
    {
        var r = await _http.GetAsync("api/branches");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<Branch>>() ?? Enumerable.Empty<Branch>();
    }

    private void SetJwtToken(string? token)
    {
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
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
}
