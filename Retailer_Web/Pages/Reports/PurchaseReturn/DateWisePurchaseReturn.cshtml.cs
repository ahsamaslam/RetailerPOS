using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.Api.Helpers;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Report.PurchaseReturn
{
    [Authorize]
    public class DateWisePurchaseReturnModel : PageModel
    {
        private readonly IApiClient _api;

        public DateWisePurchaseReturnModel(IApiClient api)
        {
            _api = api;
        }

        [BindProperty]
        public string? SelectedAction { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime? edate { get; set; }

        public List<PurchaseReturnViewModel> PurchaseReturns { get; set; } = new();
        public List<string> Actions { get; } = new() { "Excel", "PDF", "Word" };

        public async Task<IActionResult> OnGet(string? action)
        {
            await LoadPurchaseReturns();
            if (!string.IsNullOrWhiteSpace(action))
            {
                return await OnGetExportAsync(action);
            }

            return Page();
        }

        private async Task LoadPurchaseReturns()
        {
            if (sdate.HasValue && edate.HasValue)
            {
                PurchaseReturns = await _api.GetPurchaseReturnDateWiseAsync(sdate.Value, edate.Value);
            }
            else
            {
                PurchaseReturns = new List<PurchaseReturnViewModel>();
            }
        }

        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            if (!sdate.HasValue || !edate.HasValue)
            {
                return BadRequest("Dates required.");
            }

            var bytes = await _api.ExportPurchaseReturnDateWiseAsync(export, sdate.Value, edate.Value);
            var file = ExportFileResolver.Resolve(export, "PurchaseReturnReport");
            return File(bytes, file.ContentType, file.FileName);
        }
    }
}
