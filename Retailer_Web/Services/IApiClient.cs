using Microsoft.AspNetCore.Http;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.Web;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;
using Retailer.Web.Models.Ledger;
using Retailer.Web.ReportDto;
using System;

namespace Retailer.POS.Web.Services
{
    public record ApiResult(bool Success, string? ErrorMessage = null);
    public record ApiResult<T>(bool Success, T? Data = default, string? ErrorMessage = null) : ApiResult(Success, ErrorMessage);

    public interface IApiClient
    {
        Task<byte[]> ItemCsvExport();
        Task<byte[]> ExportPurchaseDateWiseAsync(string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportPurchaseItemWiseAsync(int ItemID, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportPurchaseVendorWiseAsync(int VendorID,  string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportPurchaseReturnDateWiseAsync(string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportPurchaseReturnItemWiseAsync(int itemId, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportPurchaseReturnVendorWiseAsync(int vendorId, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesDateWiseAsync(string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesItemWiseAsync(int ItemID, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesCustomerWiseAsync(int VendorID, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesReturnDateWiseAsync(string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesReturnItemWiseAsync(int itemId, string export, DateTime sdate, DateTime edate);
        Task<byte[]> ExportSalesReturnCustomerWiseAsync(int vendorId, string export, DateTime sdate, DateTime edate);
        Task<T> GetAsync<T>(string url);
        Task<T> GetAuthAsync<T>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body);
        Task DeleteAsync(string url);
        Task<List<ItemDto>> GetItemsAsync();
        Task<List<ItemDto>> SearchItemsAsync(string? term, int take = 20);
        Task<List<CitiesDto>> GetCitiesAsync();
        Task<IEnumerable<CustomerLedgerDto>> GetCustomerLedgerAsync(DateTime sdate, DateTime edate, int customerCode);
        Task<IEnumerable<ItemLedgerDto>> GetItemLedgerAsync(DateTime sdate, DateTime edate, int customerCode);
        Task<IEnumerable<VendorLedgerDto>> GetVendorLedgerAsync(DateTime sdate, DateTime edate, int customerCode);
        Task<IEnumerable<CompanyDto>> GetCompanyAsync();
        Task<CompanyDto?> GetUserCompanyAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
        Task<(bool Success, string Message, CompanyDto? Company)> CreateCompanyAsync(CompanyDto dto);
        Task<(bool Success, string Message)> UpdateCompanyAsync(Guid id, CompanyDto dto);
        Task<List<ItemDto>> GetStockItemsAsync(int categoryId = 0, int groupId = 0);
        Task<ItemDto?> GetItemAsync(int id);
        Task<(bool Success, string Message)> CreateItemAsync(CreateItemDto dto);
        Task<bool> UpdateItemAsync(ItemDto dto);
        Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
        Task<PurchaseReturnMasterDto> CreatePurchaseReturnAsync(CreatePurchaseReturnDto dto);
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
        Task<SaleInvoiceSettingDto?> GetSalePrintSetting(int id);
        Task<List<SaleInvoiceSettingDto>?> GetSalePrintSettingList();
        Task<SalesMasterReturnDto?> GetSaleReturnByIdAsync(int id);
        Task<CustomerPaymentViewModel?> GetCustomerpaymentByIdAsync(int id);
        Task<VendorPaymentViewModel?> GetVendorpaymentByIdAsync(int id);
        Task<PurchaseMasterDto?> GetPurchaseByIdAsync(int id);
        Task<PurchaseReturnMasterDto?> GetPurchaseReturnByIdAsync(int id);
        Task<SalesMasterDto?> CreateSaleAsync(SalesMasterDto dto);
        Task<SalesMasterReturnDto?> CreateSaleAsync(SalesMasterReturnDto dto);
        Task<bool> UpdateSaleAsync(SalesMasterDto dto);
        Task<bool> UpdateSaleReturnAsync(SalesMasterReturnDto dto);
        Task<bool> UpdateCustomerPaymentAsync(CustomerPaymentDto dto);
        Task<bool> UpdateVendorPaymentAsync(VendorPaymentDto dto);
        Task<bool> UpdatePurchaseAsync(PurchaseMasterDto dto);
        Task<bool> UpdatePurchaseReturnAsync(PurchaseReturnMasterDto dto);
        Task<bool> DeleteSaleAsync(int id);
        Task<bool> DeleteSaleReturnAsync(int id);
        Task<bool> DeleteCustomerPaymentAsync(int id);
        Task<bool> DeleteVendorPaymentAsync(int id);
        Task<bool> DeletePurchaseAsync(int id);
        Task<bool> DeletePurchaseReturnAsync(int id);

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
        Task<List<PurchaseViewModel>> GetPurchaseVendorWiseAsync(int vendorID , DateTime sdate, DateTime edate);
        Task<List<ItemPurchaseReport>> GetPurchaseItemWiseAsync(int itemID, DateTime sdate, DateTime edate);
        Task<List<PurchaseReturnViewModel>> GetPurchaseReturnDateWiseAsync(DateTime sdate, DateTime edate);
        Task<List<PurchaseReturnViewModel>> GetPurchaseReturnVendorWiseAsync(int vendorID , DateTime sdate, DateTime edate);
        Task<List<ItemPurchaseReport>> GetPurchaseReturnItemWiseAsync(int itemID, DateTime sdate, DateTime edate);
        Task<List<SalesViewModel>> GetSalesDateWiseAsync(DateTime sdate, DateTime edate);
        Task<List<SalesViewModel>> GetSalesCustomerWiseAsync(int vendorID, DateTime sdate, DateTime edate);
        Task<List<ItemSalesReport>> GetSalesItemWiseAsync(int itemID, DateTime sdate, DateTime edate);
        Task<List<SalesReturnViewModel>> GetSalesReturnDateWiseAsync(DateTime sdate, DateTime edate);
        Task<List<SalesReturnViewModel>> GetSalesReturnCustomerWiseAsync(int vendorID, DateTime sdate, DateTime edate);
        Task<List<ItemSalesReturnReport>> GetSalesReturnItemWiseAsync(int itemID, DateTime sdate, DateTime edate);
        Task<List<PurchaseViewModel>> GetPurchasesAsync();
        Task<List<ItemSubGroupViewModel>> GetSubGroupsAsync();
        Task<ItemSubGroupViewModel?> GetSubGroupAsync(int id);
        Task<bool> CreateSubGroupAsync(ItemSubGroupViewModel dto);
        Task<bool> UpdateSubGroupAsync(ItemSubGroupViewModel dto);
        Task DeleteSubGroupAsync(int id);

        Task<IEnumerable<BranchDto>> GetAllBranchesAsync();
        Task<BranchDto?> GetBranchByIdAsync(int id);
        Task<(bool Success, string Message)> CreateBranchAsync(BranchDto dto, Guid? companyId = null);
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
        // Upload data
        Task<(bool Success, UploadDataResultDto? Result, string Message)> UploadDataAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<(bool Success, UploadDataResultDto? Result, string Message)> UploadStockAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
