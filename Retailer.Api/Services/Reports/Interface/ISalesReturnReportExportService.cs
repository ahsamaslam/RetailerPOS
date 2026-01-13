namespace Retailer.Api.Services.Reports.Interface
{
    public interface ISalesReturnReportExportService
    {
        Task<byte[]> GenerateSalesReturnReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateCustomerSalesReturnReportAsync(int vendor ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateItemSalesReturnReportAsync(int item ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
    }
}
