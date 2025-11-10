using Retailer.POS.Web.Models;
using Retailer.POS.Api.DTOs;

namespace Retailer.POS.Web.Services;
public interface IApiClient
{
    Task<IEnumerable<ItemDto>> GetItemsAsync();
    Task<ItemDto?> GetItemAsync(int id);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto);
    Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
    Task<string?> LoginAsync(string username, string password);

    Task<IEnumerable<Customer>> GetCustomersAsync();
    Task<IEnumerable<Vendor>> GetVendorsAsync();
    Task<IEnumerable<Branch>> GetBranchesAsync();
    Task<IEnumerable<Employee>> GetEmployeesAsync();
}
