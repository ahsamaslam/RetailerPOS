using System.Net.Http.Json;
using Retailer.POS.Web.Models;
using Retailer.POS.Api.DTOs;
using System.Text.Json;

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

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var r = await _http.GetAsync("api/customers");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<Customer>>() ?? Enumerable.Empty<Customer>();
    }

    public async Task<IEnumerable<Vendor>> GetVendorsAsync()
    {
        var r = await _http.GetAsync("api/vendors");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<Vendor>>() ?? Enumerable.Empty<Vendor>();
    }

    public async Task<IEnumerable<Branch>> GetBranchesAsync()
    {
        var r = await _http.GetAsync("api/branches");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<Branch>>() ?? Enumerable.Empty<Branch>();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        var r = await _http.GetAsync("api/employees");
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<IEnumerable<Employee>>() ?? Enumerable.Empty<Employee>();
    }
}
