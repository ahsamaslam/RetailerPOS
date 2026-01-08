using System.Data;

namespace Retailer.Api.Services.Reports.Interface
{
    public interface IReportGeneratorService
    {
        Task<byte[]> GenerateAsync(
        string rdlcFileName,
        IDictionary<string, DataTable> datasets,
         IDictionary<string, object>? parameters,
        string exportType);
    }
}
