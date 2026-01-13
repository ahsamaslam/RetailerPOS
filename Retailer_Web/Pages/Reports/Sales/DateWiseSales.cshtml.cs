using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.Api.Helpers;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Report.Sales
{
    [Authorize]
    public class DateWiseSalesModel : PageModel
    {
        private readonly IApiClient _api;

        public DateWiseSalesModel(IApiClient api)
        {
            _api = api;
        }

        [BindProperty]
        public string? SelectedAction { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime? edate { get; set; }

        public List<SalesViewModel> Sales { get; set; } = new();
        public List<string> Actions { get; } = new() { "Excel", "PDF", "Word" };

        public async Task<IActionResult> OnGet(string? action)
        {
            await LoadSales();
            if (!string.IsNullOrWhiteSpace(action))
            {
                return await OnGetExportAsync(action);
            }

            return Page();
        }

        private async Task LoadSales()
        {
            if (sdate.HasValue && edate.HasValue)
            {
                Sales = await _api.GetSalesDateWiseAsync(sdate.Value, edate.Value);
            }
            else
            {
                Sales = new List<SalesViewModel>();
            }
        }

        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            if (!sdate.HasValue || !edate.HasValue)
            {
                return BadRequest("Dates required.");
            }

            var bytes = await _api.ExportSalesDateWiseAsync(export, sdate.Value, edate.Value);
            var file = ExportFileResolver.Resolve(export, "PurchaseReturnReport");
            return File(bytes, file.ContentType, file.FileName);
        }
    }
}
