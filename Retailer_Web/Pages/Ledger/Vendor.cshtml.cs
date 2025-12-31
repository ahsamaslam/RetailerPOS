using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models.Ledger;

namespace Retailer.Web.Pages.Ledger
{
    public class VendorModel : PageModel
    {

		private readonly IWebHostEnvironment _env;

		private readonly IApiClient _api;
		public VendorModel(IApiClient api) { _api = api; }

		public int vendorCode { get; set; }
		public VendorDto Vendor { get; set; }
		public List<VendorLedgerDto> ledgers { get; set; } = new();
		public DateTime sdate { get; set; } = DateTime.Now;
		public DateTime edate { get; set; } = DateTime.Now;
		public List<SelectListItem> VendorList { get; set; } = new();

		public async Task<IActionResult> OnGetAsync(DateTime? sdate, DateTime? edate, int status)
		{


			var customers = await _api.GetVendorsAsync();

			VendorList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

			return Page();
		}
		 
		public async Task<IActionResult> OnGetLoadDataAsync(
			int vendorCode,
			DateTime sdate,
			DateTime edate)
		{
			var data = await _api.GetVendorLedgerAsync(sdate, edate, vendorCode);
			return new JsonResult(data);
		}
	}
}
