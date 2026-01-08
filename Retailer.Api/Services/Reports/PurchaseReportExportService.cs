using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Reporting.NETCore;
using Retailer.Api.Helper;
using Retailer.Api.Services.Reports.Interface;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Services;
using System.Data;
using System.Numerics;

namespace Retailer.Api.Services.Reports
{
    public class PurchaseReportExportService : IPurchaseReportExportService
    {


        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IReportGeneratorService _reportS;
        private readonly ICompanyService _companyService;
        private readonly IPurchaseService _purchaseService;
        public PurchaseReportExportService(
            RetailerDbContext db,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory,
            IPurchaseService purchaseService,
            IReportGeneratorService reportS,
            ICompanyService companyService)
        {
            _db = db;
            _cache = cache;
            _reportS = reportS;
            _companyService = companyService;
            _purchaseService = purchaseService;
            // get the named client
            _httpClient = httpClientFactory.CreateClient("AuthModule");
        }

        public async Task<byte[]> GenerateItemPurchaseReportAsync(int item, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {

                Item ven = await _db.Items.Where(r => r.Id == item).FirstOrDefaultAsync();
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                var purchases = await _purchaseService.GetItemWiseAsync(item, sdate, edate, CompanyId);

                var datasets = new Dictionary<string, DataTable>
        {
            { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
            { "DataSet2", DataTableHelper.ItemWisePurchaseToDataTable(purchases.ToList()) }
        };
                var parameters = new Dictionary<string, object>
{
    { "sdate", sdate.ToString("dd MMM yyyy") },
    { "edate", edate.ToString("dd MMM yyyy") },
    { "Name", ven.Name }
};
                return await _reportS.GenerateAsync("DateWisePurchaseI.rdlc", datasets, parameters, export);
            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }
        public async Task<byte[]> GeneratePurchaseReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            { 
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                 var purchases = await _purchaseService.GetDateWiseAsync(sdate, edate, CompanyId);
            
                var datasets = 
                    new Dictionary<string, DataTable>
        {
            { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
            { "DataSet2", DataTableHelper.ToPurchaseDataTable(purchases.ToList()) }
        };
                var parameters = new Dictionary<string, object>
{
    { "sdate",  sdate.ToString("dd MMM yyyy") },
    { "edate", edate .ToString("dd MMM yyyy")}, 
};

                return await _reportS.GenerateAsync("DateWisePurchase.rdlc", datasets, parameters, export);
               
            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }

        public async  Task<byte[]> GenerateVendorPurchaseReportAsync(int vendor, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {

                Vendor ven =await _db.Vendors.Where(r => r.Id == vendor).FirstOrDefaultAsync();
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                var purchases = await _purchaseService.GetVendorWiseAsync(vendor,sdate, edate, CompanyId);

                var datasets = new Dictionary<string, DataTable>
        {
            { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
            { "DataSet2", DataTableHelper.ToPurchaseDataTable(purchases.ToList()) }
        };
                var parameters = new Dictionary<string, object>
{
    { "sdate", sdate.ToString("dd MMM yyyy") },
    { "edate", edate.ToString("dd MMM yyyy") },
    { "vendorName", ven.Name }
};
                return await _reportS.GenerateAsync("DateWisePurchaseV.rdlc", datasets, parameters, export);

            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }
    }
}
