namespace Retailer.Api.Services.Reports.Interface
{
    public interface IPurchaseReturnReportExportService
    {
        Task<byte[]> GeneratePurchaseReturnReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateVendorPurchaseReturnReportAsync(int vendorId, DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateItemPurchaseReturnReportAsync(int itemId, DateTime sdate, DateTime edate, Guid CompanyId, string export);
    }
}
