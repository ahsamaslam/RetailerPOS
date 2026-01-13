using Microsoft.EntityFrameworkCore;
using Retailer.Api.Helper;
using Retailer.Api.Services.Reports.Interface;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;
using System.Data;

namespace Retailer.Api.Services.Reports
{
    public class PurchaseReturnReportExportService : IPurchaseReturnReportExportService
    {
        private readonly RetailerDbContext _db;
        private readonly IPurchaseReturnService _purchaseReturnService;
        private readonly IReportGeneratorService _reportGenerator;
        private readonly ICompanyService _companyService;

        public PurchaseReturnReportExportService(
            RetailerDbContext db,
            IPurchaseReturnService purchaseReturnService,
            IReportGeneratorService reportGenerator,
            ICompanyService companyService)
        {
            _db = db;
            _purchaseReturnService = purchaseReturnService;
            _reportGenerator = reportGenerator;
            _companyService = companyService;
        }

        public async Task<byte[]> GeneratePurchaseReturnReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(CompanyId);
                var returns = (await _purchaseReturnService.GetDateWiseAsync(sdate, edate, CompanyId))
                    .Where(r => r != null)
                    .Select(r => r!)
                    .ToList();

                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ToPurchaseReturnDataTable(returns) }
                };

                var parameters = BuildDateRangeParameters(sdate, edate);

                return await _reportGenerator.GenerateAsync("DateWisePurchaseReturn.rdlc", datasets, parameters, export);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GenerateVendorPurchaseReturnReportAsync(int vendorId, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {
                var vendor = await _db.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId);
                var company = await _companyService.GetCompanyByIdAsync(CompanyId);
                var returns = (await _purchaseReturnService.GetVendorWiseAsync(vendorId, sdate, edate, CompanyId))
                    .Where(r => r != null)
                    .Select(r => r!)
                    .ToList();

                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ToPurchaseReturnDataTable(returns) }
                };

                var parameters = BuildDateRangeParameters(sdate, edate);
                parameters["vendorName"] = vendor?.Name ?? string.Empty;

                return await _reportGenerator.GenerateAsync("DateWisePurchaseReturnV.rdlc", datasets, parameters, export);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GenerateItemPurchaseReturnReportAsync(int itemId, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {
                var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == itemId);
                var company = await _companyService.GetCompanyByIdAsync(CompanyId);
                var details = (await _purchaseReturnService.GetItemWiseAsync(itemId, sdate, edate, CompanyId))
                    .Where(r => r != null)
                    .Select(r => r!)
                    .ToList();

                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ItemWisePurchaseReturnToDataTable(details) }
                };

                var parameters = BuildDateRangeParameters(sdate, edate);
                parameters["Name"] = item?.Name ?? string.Empty;

                return await _reportGenerator.GenerateAsync("DateWisePurchaseReturnI.rdlc", datasets, parameters, export);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        private static Dictionary<string, object> BuildDateRangeParameters(DateTime sdate, DateTime edate) => new()
        {
            { "sdate", sdate.ToString("dd MMM yyyy") },
            { "edate", edate.ToString("dd MMM yyyy") }
        };
    }
}
