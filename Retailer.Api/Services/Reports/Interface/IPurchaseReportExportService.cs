namespace Retailer.Api.Services.Reports.Interface
{
    public interface IPurchaseReportExportService
    {
        Task<byte[]> GeneratePurchaseReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateVendorPurchaseReportAsync(int vendor ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateItemPurchaseReportAsync(int item ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
    }
}
