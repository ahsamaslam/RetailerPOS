using Retailer.POS.Web.Models;
using Retailer.Web.Models;
using Retailer.POS.Web.DTOs;

namespace Retailer.POS.Web.Services;
public interface IApiClient
{
    Task<List<ItemDto>> GetItemsAsync();
    Task<ItemDto?> GetItemAsync(int id);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto);
    Task<bool> UpdateItemAsync(ItemDto dto);
    Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
    Task<string?> LoginAsync(string username, string password);
    Task<List<Branch>> GetBranchesAsync();
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
    // Category
    Task<List<ItemCategoryViewModel>> GetCategoriesAsync();
    Task<ItemCategoryViewModel?> GetCategoryAsync(int id);
    Task<bool> CreateCategoryAsync(ItemCategoryViewModel dto);
    Task UpdateCategoryAsync(ItemCategoryViewModel dto);
    Task DeleteCategoryAsync(int id);

    // Group
    Task<List<ItemGroupViewModel>> GetGroupsAsync();
    Task<ItemGroupViewModel?> GetGroupAsync(int id);
    Task<bool> CreateGroupAsync(ItemGroupViewModel dto);
    Task<bool> UpdateGroupAsync(ItemGroupViewModel dto);
    Task DeleteGroupAsync(int id);

    // SubGroup
    Task<List<ItemSubGroupViewModel>> GetSubGroupsAsync();
    Task<ItemSubGroupViewModel?> GetSubGroupAsync(int id);
    Task<bool> CreateSubGroupAsync(ItemSubGroupViewModel dto);
    Task<bool> UpdateSubGroupAsync(ItemSubGroupViewModel dto);
    Task DeleteSubGroupAsync(int id);

}
