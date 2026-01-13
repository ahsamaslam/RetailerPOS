using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.Api.Helpers;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.ReportDto;

namespace Retailer.Web.Pages.Report.Purchase
{
    public class ItemWisePurchaseModel : PageModel
    {
        private readonly IApiClient _api;

        public ItemWisePurchaseModel(IApiClient api)
        {
            _api = api;
        }
        [BindProperty(SupportsGet = true)]
        public bool FirstLoad { get; set; }
        [BindProperty]
        public string? SelectedAction { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime? edate { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? ItemCode { get; set; }

        public List<SelectListItem> Items { get; set; } = new();
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
        // ✅ INITIAL PAGE LOAD
        public async Task<IActionResult> OnGet()
        {
            await LoadItems();
            return Page();
        }

        public async Task LoadItems()
        {
            var Item = await _api.GetItemsAsync();
            Items = Item.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();


        }
        // ✅ AJAX HANDLER (IMPORTANT)
        public async Task<IActionResult> OnGetLoadPurchase(
            int? ItemCode,
            DateTime? sdate,
            DateTime? edate)
        {
            if (!ItemCode.HasValue || !sdate.HasValue || !edate.HasValue)
                return new JsonResult(new List<ItemPurchaseReport>());

            var data = await _api.GetPurchaseItemWiseAsync(
                ItemCode.Value,
                sdate.Value,
                edate.Value);

            return new JsonResult(data);
        }

        // ✅ EXPORT
        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            if (!sdate.HasValue || !edate.HasValue)
                return BadRequest("Dates required.");

            if (!ItemCode.HasValue)
                return BadRequest("Item required.");

            var bytes = await _api.ExportPurchaseItemWiseAsync(
              ItemCode.Value, export, sdate.Value, edate.Value);

            var file = ExportFileResolver.Resolve(export, "PurchaseReport");
            return File(bytes, file.ContentType, file.FileName);
        }
    }
}