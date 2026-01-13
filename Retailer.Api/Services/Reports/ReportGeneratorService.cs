using Microsoft.Reporting.NETCore;
using Retailer.Api.Services.Reports.Interface;
using System.Data;

public class ReportGeneratorService : IReportGeneratorService
{
    private readonly IWebHostEnvironment _env;

    public ReportGeneratorService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<byte[]> GenerateAsync(
        string rdlcFileName,
        IDictionary<string, DataTable> datasets,
         IDictionary<string, object>? parameters,
        string exportType)
    {
        exportType = exportType.ToLower();
        var reportPath = Path.Combine(_env.ContentRootPath, "Reports", rdlcFileName);

        if (!File.Exists(reportPath))
            throw new FileNotFoundException("RDLC file not found", reportPath);

        var localReport = new LocalReport
        {
            ReportPath = reportPath
        };

        localReport.DataSources.Clear();

        foreach (var ds in datasets)
        {
            localReport.DataSources.Add(new ReportDataSource(ds.Key, ds.Value));
        }
        if (parameters != null && parameters.Any())
        {
            var reportParams = parameters
                .Select(p => new ReportParameter(p.Key, p.Value?.ToString() ?? ""))
                .ToList();

            localReport.SetParameters(reportParams);
        }
        string mimeType, encoding, fileNameExtension;
        Warning[] warnings;
        string[] streams;

        var format = exportType.ToLower() switch
        {
            "pdf" => "PDF",
            "excel" => "EXCEL",
            "word" => "WORDOPENXML",
            _ => throw new ArgumentException("Invalid export type")
        };

        var result = localReport.Render(
            format,
            null,
            out mimeType,
            out encoding,
            out fileNameExtension,
            out streams,
            out warnings
        );

        return await Task.FromResult(result);
    }
}
