namespace Retailer.Api.Services.Reports.Interface
{
    public interface ISalesReportExportService
    {
        Task<byte[]> GenerateSaleseReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateCustomerSalesReportAsync(int customer ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
        Task<byte[]> GenerateItemSalesReportAsync(int item ,  DateTime sdate, DateTime edate, Guid CompanyId, string export);
    }
}
