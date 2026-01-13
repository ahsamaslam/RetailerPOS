using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using Retailer.Web.Pages;
using System.Reflection;

namespace Retailer.POS.Web.Pages.Sales
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        private readonly RdlcDataTableHelper _rdlcHelper;


        private readonly IWebHostEnvironment _env;
 
        private readonly IApiClient _api;
        public IndexModel(IApiClient api, IWebHostEnvironment env, RdlcDataTableHelper rdlcHelper) 
        {
            _api = api;
            _env = env;
            _rdlcHelper = rdlcHelper;
        }
        [BindProperty(SupportsGet = true)]
        public DateTime sdate { get; set; } = DateTime.Now; 

        [BindProperty(SupportsGet = true)]
        public DateTime edate { get; set; } = DateTime.Now;
        [BindProperty]
        public List<SalesMasterDto> Sales { get; set; } = new();
        [BindProperty]
        public CompanyViewModel Company { get; set; } = new();
        [BindProperty]
        public IFormFile? LogoFile { get; set; } // For new file upload 
        public string? logoPath { get; set; } // For new file upload

        public async Task<IFormFile?> GetIFormFileFromUrlAsync(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var contentStream = await response.Content.ReadAsStreamAsync();

            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            // Derive filename from URL (or set your own)
            var fileName = Path.GetFileName(new Uri(url).AbsolutePath);

            // Create the IFormFile from memory
            var formFile = new FormFile(
                baseStream: new MemoryStream(contentBytes),
                baseStreamOffset: 0,
                length: contentBytes.Length,
                name: "file",
                fileName: fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream"
            };

            return formFile;
        }
        public async Task< CompanyViewModel> getActiveCompany() {

            var company = await _api.GetUserCompanyAsync();

            if (company != null)
            {
                Company = new CompanyViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    ContactPhone = company.ContactPhone,
                    ContactEmail = company.ContactEmail,
                    NTN = company.NTN,
                    CNIC = company.CNIC,
                    STRN = company.STRN,
                    ContactPerson = company.ContactPerson,
                    ShortName = company.ShortName,
                    fbrToken = company.fbrToken,
                    pralToken = company.pralToken,
                    fbrActive = company.fbrActive,
                    Province = company.Province,
                    logoPath = company.logoPath,
                    CompanyType = company.CompanyType,
                    isEd = company.isEd,
                    isFed = company.isFed,
                    isGst = company.isGst,
                    gstVal = company.gstVal,
                    fedVal = company.fedVal,
                    edVal = company.edVal

                };
                if (company.logoPath != null)
                {
                    //Company.img = System.IO.File.ReadAllBytes(_rdlcHelper.getFullpathCompanyLogo(_env, company.logoPath.ToString()));
                    LogoFile = await GetIFormFileFromUrlAsync(company.logoPath);
                    logoPath = company.logoPath;
                }

            }
            return Company; 
        }
      
        public async Task OnGetAsync()
        {
            //Company=await getActiveCompany();

            #region sales
             Sales = (await _api.GetAllSaleDateWise(sdate, edate)).ToList();
            #endregion
            // Sales = (await _api.GetAllSaleDateWise(sdate,edate)).ToList();
        }
        public async Task< IActionResult> OnPostCancelSale(int id)
        {

            var aa = await _api.DeleteSaleAsync(id);
            //// Find the sale by id
            //var sale = Sales.FirstOrDefault(s => s.Id == id);
            //if (sale != null)
            //{
            //    // Example: mark as canceled or delete
            //    sale.Status = "Canceled";
            //    // Or remove from list: Sales.Remove(sale);
            //}

            // Refresh the page to show updated data
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPrint(int id)
        {
            try
            {
              var   company = await getActiveCompany();
                var csale = await _api.GetSaleByIdAsync(id);

                object imgValue = company.logoPath != null ? _rdlcHelper.GetImageBytesFromPath(_env, company.logoPath) : DBNull.Value;
                
              
                var reportPath = Path.Combine(
               _env.WebRootPath,
               "Report",
               "saleInvoice.rdlc"
           );

                var report = new LocalReport();
                report.ReportPath = reportPath; 
                // 🔹 DataSet1 (Company)
                report.DataSources.Add(
                    new ReportDataSource("DataSet1",    _rdlcHelper.CompanyToDataTable(company, imgValue))
                );

                // 🔹 DataSet2 (Invoice Summary)
                report.DataSources.Add(
                    new ReportDataSource("DataSet2", _rdlcHelper.InvoiceSummaryToDataTable(csale))
                );

                // 🔹 DataSet3 (Invoice Items)
                report.DataSources.Add(
                    new ReportDataSource("DataSet3", _rdlcHelper.InvoiceItemsToDataTable(csale.Details))
                );
 

                var pdf = report.Render("PDF");

                return File(pdf, "application/pdf", "Invoice.pdf");
            }
            catch (Exception exx)
            {
                //_logger.LogError(ex, "Failed to generate report");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred."
                );
            }
            //      Sales = (await _api.GetAllSaleDateWise(sdate, edate)).ToList();
            // Sales = (await _api.GetAllSaleDateWise(sdate,edate)).ToList();
        }
    }
}
