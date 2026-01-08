using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailer.Api.Helper;
using Retailer.Api.Services.Reports.Interface;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Services;
using System.Data;

namespace Retailer.Api.Services
{
    public class SalesReturnReportExportService : ISalesReturnReportExportService
    {


        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IReportGeneratorService _reportS;
        private readonly ICompanyService _companyService;
        private readonly ISalesReturnService _SalesService;
        public SalesReturnReportExportService(
            RetailerDbContext db,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory,
            ISalesReturnService SalesService,
            IReportGeneratorService reportS,
            ICompanyService companyService)
        {
            _db = db;
            _cache = cache;
            _reportS = reportS;
            _companyService = companyService;
            _SalesService = SalesService;
            // get the named client
            _httpClient = httpClientFactory.CreateClient("AuthModule");
        }

        public async Task<byte[]> GenerateItemSalesReturnReportAsync(int item, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {

                Item ven = await _db.Items.Where(r => r.Id == item).FirstOrDefaultAsync();
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                var Sales = await _SalesService.GetItemWiseAsync(item, sdate, edate, CompanyId);

                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ItemWiseSalesReturnToDataTable(Sales.ToList()) }
                };
                var parameters = new Dictionary<string, object>
                {
                    { "sdate", sdate.ToString("dd MMM yyyy") },
                    { "edate", edate.ToString("dd MMM yyyy") },
                    { "Name", ven.Name }
                };
                return await _reportS.GenerateAsync("DateWiseSalesI.rdlc", datasets, parameters, export);
            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }
        public async Task<byte[]> GenerateSalesReturnReportAsync(DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                var Sales = await _SalesService.GetDateWiseAsync(sdate, edate, CompanyId);
                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ToSalesReturnDataTable(Sales.ToList()) }
                };
                var parameters = new Dictionary<string, object>
                {
                    { "sdate",  sdate.ToString("dd MMM yyyy") },
                    { "edate", edate .ToString("dd MMM yyyy")},
                };
                return await _reportS.GenerateAsync("DateWiseSales.rdlc", datasets, parameters, export);
            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GenerateCustomerSalesReturnReportAsync(int customer, DateTime sdate, DateTime edate, Guid CompanyId, string export)
        {
            try
            {

                Vendor ven = await _db.Vendors.Where(r => r.Id == customer).FirstOrDefaultAsync();
                var company = await _companyService.GetCompanyByIdAsync((CompanyId));
                var Saless = await _SalesService.GetCustomerWiseAsync(customer, sdate, edate, CompanyId);

                var datasets = new Dictionary<string, DataTable>
                {
                    { "DataSet1", await DataTableHelper.CompanyToDataTable(company) },
                    { "DataSet2", DataTableHelper.ToSalesReturnDataTable(Saless.ToList()) }
                };
                var parameters = new Dictionary<string, object>
                {
                    { "sdate", sdate.ToString("dd MMM yyyy") },
                    { "edate", edate.ToString("dd MMM yyyy") },
                    { "vendorName", ven.Name }
                };
                return await _reportS.GenerateAsync("DateWiseSalesV.rdlc", datasets, parameters, export);

            }
            catch (Exception exx)
            {
                return Array.Empty<byte>();
            }
        }

    }
}
