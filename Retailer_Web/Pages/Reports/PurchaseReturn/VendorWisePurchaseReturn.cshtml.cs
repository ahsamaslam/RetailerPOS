using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.Api.Helpers;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Report.PurchaseReturn
{
    [Authorize]
    public class VendorWisePurchaseReturnModel : PageModel
    {
        private readonly IApiClient _api;

        public VendorWisePurchaseReturnModel(IApiClient api)
        {
            _api = api;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime? edate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VendorCode { get; set; }

        public List<SelectListItem> Vendors { get; set; } = new();
        public List<string> Actions { get; } = new() { "Excel", "PDF", "Word" };

        public async Task<IActionResult> OnGet()
        {
            await LoadVendors();
            return Page();
        }

        private async Task LoadVendors()
        {
            var vendor = await _api.GetVendorsAsync();
            Vendors = vendor.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        }

        public async Task<IActionResult> OnGetLoadPurchaseReturn(int? vendorCode, DateTime? sdate, DateTime? edate)
        {
            if (!vendorCode.HasValue || !sdate.HasValue || !edate.HasValue)
            {
                return new JsonResult(new List<PurchaseReturnViewModel>());
            }

            var data = await _api.GetPurchaseReturnVendorWiseAsync(vendorCode.Value, sdate.Value, edate.Value);
            return new JsonResult(data);
        }

        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            if (!sdate.HasValue || !edate.HasValue)
            {
                return BadRequest("Dates required.");
            }

            if (!VendorCode.HasValue)
            {
                return BadRequest("Vendor required.");
            }

            var bytes = await _api.ExportPurchaseReturnVendorWiseAsync(VendorCode.Value, export, sdate.Value, edate.Value);
            var file = ExportFileResolver.Resolve(export, "PurchaseReturnReport");
            return File(bytes, file.ContentType, file.FileName);
        }
    }
}
