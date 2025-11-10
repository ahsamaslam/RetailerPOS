using Retailer.POS.Web.Models;
using Retailer.POS.Api.DTOs;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Services;
public interface IApiClient
{
    Task<IEnumerable<ItemDto>> GetItemsAsync();
    Task<ItemDto?> GetItemAsync(int id);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto);
    Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
    Task<string?> LoginAsync(string username, string password);
    Task<IEnumerable<Branch>> GetBranchesAsync();
    Task<List<EmployeeViewModel>> GetEmployeesAsync();
    Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id);
    Task<bool> CreateEmployeeAsync(EmployeeViewModel employee);
    Task<bool> UpdateEmployeeAsync(EmployeeViewModel employee);

    Task<List<CustomerViewModel>> GetCustomersAsync();
    Task<CustomerViewModel?> GetCustomerByIdAsync(int id);
    Task<bool> CreateCustomerAsync(CustomerViewModel customer);
    Task<bool> UpdateCustomerAsync(CustomerViewModel customer);

    Task<List<VendorViewModel>> GetVendorsAsync();
    Task<VendorViewModel?> GetVendorByIdAsync(int id);
    Task<bool> CreateVendorAsync(VendorViewModel vendor);
    Task<bool> UpdateVendorAsync(VendorViewModel vendor);

    Task<bool> CreateSalesAsync(SalesViewModel sale);
    Task<SalesViewModel?> GetSaleByIdAsync(int id);
    Task UpdateSaleAsync(SalesViewModel sale);
    Task DeleteSaleAsync(int id);
    Task<List<SalesViewModel>> GetSalesAsync();

}
