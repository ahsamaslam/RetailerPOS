using Microsoft.AspNetCore.Http;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.Web;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;
using Retailer.Web.Models.Ledger;

namespace Retailer.POS.Web.Services
{
    public record ApiResult(bool Success, string? ErrorMessage = null);
    public record ApiResult<T>(bool Success, T? Data = default, string? ErrorMessage = null) : ApiResult(Success, ErrorMessage);

    public interface IApiClient
    {
        Task<T> GetAsync<T>(string url);
        Task<T> GetAuthAsync<T>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body);
        Task DeleteAsync(string url);
        Task<List<ItemDto>> GetItemsAsync();
        Task<List<CitiesDto>> GetCitiesAsync();
        Task<IEnumerable<CustomerLedgerDto>> GetCustomerLedgerAsync(DateTime sdate, DateTime edate, int customerCode);
        Task<CompanyDto?> GetCompanyAsync();
        Task<CompanyDto?> GetUserCompanyAsync();
        Task<CompanyDto?> GetCompanybyIdAsync(string guid);
        Task<List<ItemDto>> GetStockItemsAsync(int categoryId = 0, int groupId = 0);
        Task<ItemDto?> GetItemAsync(int id);
        Task<(bool Success, string Message)> CreateItemAsync(CreateItemDto dto);
        Task<bool> UpdateItemAsync(ItemDto dto);
        Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
        Task<string?> LoginAsync(string username, string password);
        Task<List<EmployeeDto>> GetEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<CustomerPaymentDto>> GetAllCustomerPaymentDateWise(DateTime sdate, DateTime edate);
        Task<IEnumerable<VendorPaymentDto>> GetAllVendorPaymentDateWise(DateTime sdate, DateTime edate);
        Task<bool> CreateEmployeeAsync(EmployeeDto employee);
        Task<bool> CreateCustomerPaymentAsync(CustomerPaymentViewModel customer);
        Task<bool> CreateVendorPaymentAsync(VendorPaymentViewModel customer);
        Task<bool> UpdateEmployeeAsync(EmployeeDto employee);
        Task<List<ProvienceDto>> GetProvienceAsync();
        Task<double> GetCustomersBalanceAsync(DateTime edate, int customerCode);
        Task<double> GetVendorBalanceAsync(DateTime edate, int customerCode);
        Task<List<CustomerViewModel>> GetCustomersAsync();
        Task<List<PaymentMethodDto>> GetPaymentMethodAsync();
        Task<List<BanksViewModel>> GetBanksAsync();
        Task<CustomerViewModel?> GetCustomerByIdAsync(int id);
        Task<BanksViewModel?> GetBankByIdAsync(int id);
        Task<bool> CreateCustomerAsync(CustomerViewModel customer);
        Task<bool> CreateBankAsync(BanksViewModel customer);
        Task<bool> UpdateCustomerAsync(CustomerViewModel customer);
        Task<bool> UpdateBankAsync(BanksViewModel bank);

        Task<List<VendorViewModel>> GetVendorsAsync();
        Task<VendorViewModel?> GetVendorByIdAsync(int id);
        Task<bool> CreateVendorAsync(VendorViewModel vendor);
        Task<bool> UpdateVendorAsync(VendorViewModel vendor);

        // Sales
        Task<IEnumerable<SalesMasterDto>> GetAllSaleDateWise(DateTime sdate, DateTime edate);
        Task<IEnumerable<SalesMasterReturnDto>> GetAllSaleReturnDateWise(DateTime sdate, DateTime edate);
        Task<IEnumerable<SalesMasterDto>> GetSalesAsync();
        Task<SalesMasterDto?> GetSaleByIdAsync(int id);
        Task<SalesMasterReturnDto?> GetSaleReturnByIdAsync(int id);
        Task<CustomerPaymentViewModel?> GetCustomerpaymentByIdAsync(int id);
        Task<VendorPaymentViewModel?> GetVendorpaymentByIdAsync(int id);
        Task<PurchaseMasterDto?> GetPurchaseByIdAsync(int id);
        Task<SalesMasterDto?> CreateSaleAsync(SalesMasterDto dto);
        Task<SalesMasterReturnDto?> CreateSaleAsync(SalesMasterReturnDto dto);
        Task<bool> UpdateSaleAsync(SalesMasterDto dto);
        Task<bool> UpdateSaleReturnAsync(SalesMasterReturnDto dto);
        Task<bool> UpdateCustomerPaymentAsync(CustomerPaymentDto dto);
        Task<bool> UpdateVendorPaymentAsync(VendorPaymentDto dto);
        Task<bool> UpdatePurchaseAsync(PurchaseMasterDto dto);
        Task<bool> DeleteSaleAsync(int id);
        Task<bool> DeleteSaleReturnAsync(int id);
        Task<bool> DeleteCustomerPaymentAsync(int id);
        Task<bool> DeleteVendorPaymentAsync(int id);
        Task<bool> DeletePurchaseAsync(int id);

        // Category
        Task<List<ItemCategoryViewModel>> GetCategoriesAsync();
        Task<ItemCategoryViewModel?> GetCategoryAsync(int id);
        Task<ItemTypeViewModel?> GetItemTypeAsync(int id);
        Task<(bool Success, string Message)> CreateCategoryAsync(ItemCategoryViewModel dto);
        Task<(bool Success, string Message)> CreateItemTypeAsync(ItemTypeViewModel dto);
        Task UpdateCategoryAsync(ItemCategoryViewModel dto);
        Task<bool> UpdateItemTypeAsync(ItemTypeViewModel dto);
        Task DeleteCategoryAsync(int id);

        // Group
        Task<List<ItemGroupViewModel>> GetGroupsAsync();
        Task<ItemGroupViewModel?> GetGroupAsync(int id);
        Task<(bool Success, string Message)> CreateGroupAsync(ItemGroupViewModel dto);
        Task<bool> UpdateGroupAsync(ItemGroupViewModel dto);
        Task DeleteGroupAsync(int id);

        // SubGroup
        Task<List<ItemTypeViewModel>> GetItemTypeAsync();
        Task<List<PurchaseViewModel>> GetPurchaseDateWiseAsync(DateTime sdate, DateTime edate);
        Task<List<PurchaseViewModel>> GetPurchasesAsync();
        Task<List<ItemSubGroupViewModel>> GetSubGroupsAsync();
        Task<ItemSubGroupViewModel?> GetSubGroupAsync(int id);
        Task<bool> CreateSubGroupAsync(ItemSubGroupViewModel dto);
        Task<bool> UpdateSubGroupAsync(ItemSubGroupViewModel dto);
        Task DeleteSubGroupAsync(int id);

        Task<IEnumerable<BranchDto>> GetAllBranchesAsync();
        Task<BranchDto?> GetBranchByIdAsync(int id);
        Task<bool> CreateBranchAsync(BranchDto dto);
        Task<bool> UpdateBranchAsync(BranchDto dto);
        Task<bool> DeleteBranchAsync(int id);

        Task<IEnumerable<ScopeDto>> GetAllScopesAsync();
        Task<ScopeDto?> GetScopeByIdAsync(int id);
        Task<bool> CreateScopeAsync(ScopeDto dto);
        Task<bool> UpdateScopeAsync(ScopeDto dto);
        Task<bool> DeleteScopeAsync(int id);


        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<bool> CreateRoleAsync(RoleDto dto);
        Task<bool> UpdateRoleAsync(RoleDto dto);

        Task<IEnumerable<MenuDto>> GetMenusForCurrentUserAsync();

        Task<List<OpeningBalanceViewModel>> GetOpeningBalancesAsync();
        Task<OpeningBalanceViewModel?> GetOpeningBalanceAsync(int id);
        Task<ApiResult<OpeningBalanceViewModel>> CreateOpeningBalanceAsync(CreateOpeningBalanceDto dto);
        Task<ApiResult> UpdateOpeningBalanceAsync(int id, UpdateOpeningBalanceDto dto);
        Task<ApiResult> DeleteOpeningBalanceAsync(int id);


        //Menu Management
        Task<MenuDto?> CreateMenuAsync(MenuDto dto);
        Task<SubMenuDto?> CreateSubMenuAsync(int menuId, SubMenuDto dto);
        Task<bool> DeleteMenuAsync(int menuId);
        Task<bool> DeleteSubMenuAsync(int menuId, int subMenuId);
    }
}
