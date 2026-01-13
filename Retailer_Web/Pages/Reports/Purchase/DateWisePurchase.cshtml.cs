using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.Api.Helpers;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Retailer.Web.Pages.Report.Purchase
{
    [Authorize]
    public class DateWisePurchaseModel : PageModel
    {
        private readonly IApiClient _api;

        public DateWisePurchaseModel(IApiClient api)
        {
            _api = api;
        }
        [BindProperty]
        public string? SelectedAction { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime? edate { get; set; }

        public List<PurchaseViewModel> Purchase { get; set; } = new();
        public List<string> Actions { get; set; } = new() { "Excel", "PDF", "Word" };


        public IActionResult OnPostAction(string action, int id)
        {
            SelectedAction = action;
            return Page();
        }
        public IActionResult OnPostSelect(string value)
        { 
            return Page();
        }

        // 👉 Footer totals
        public decimal TotalSubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalNet { get; set; }
        public async Task<IActionResult> OnGet(DateTime? sdate, DateTime? edate, string action)
        {
            await   LoadPurchases();

            if (!string.IsNullOrEmpty(action))
            {
              return  await    OnGetExportAsync(  action);
            }

            return Page();
        }

        public async Task LoadPurchases() {
            if (sdate.HasValue && edate.HasValue)
            {
                Purchase = await _api.GetPurchaseDateWiseAsync(sdate.Value, edate.Value);
            }
            else
            {
                Purchase = new();
            } 
        }
        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            if (!sdate.HasValue || !edate.HasValue)
                return BadRequest("Dates required.");

            var bytes = await _api.ExportPurchaseDateWiseAsync(export, sdate.Value, edate.Value);
            var file = ExportFileResolver.Resolve(export, "PurchaseReport");

            return File(bytes, file.ContentType, file.FileName);
        }
       
    }
}
