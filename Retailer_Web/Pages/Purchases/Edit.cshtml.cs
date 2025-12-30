using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Purchases
{
    public class EditModel : PageModel
    {
		private readonly IApiClient _api;
		public EditModel(IApiClient api) => _api = api;

		[BindProperty]
		public PurchaseMasterDto Purchase { get; set; } = new();
		public IEnumerable<SelectListItem> ItemsList { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> vendorList { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> PurchaseType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
		, new SelectListItem { Value = "1", Text = "Credit" } };
		public async Task<IActionResult> OnGetAsync(int id)
		{
			ItemsList = (await _api.GetItemsAsync())
				.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
			vendorList = (await _api.GetVendorsAsync())
				.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
		
			Purchase = await _api.GetPurchaseByIdAsync(id); // fetch DTO from API
			if (Purchase == null) return NotFound();
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			// recompute totals server-side
			Purchase.SubTotal = Purchase.Details.Sum(d => d.Amount);
			Purchase.TaxAmount = Purchase.Details.Sum(d => d.TaxAmount);
			Purchase.TotalDiscount = Purchase.Details.Sum(d => d.Discount);
			Purchase.BalanceAmount = Purchase.SubTotal - Purchase.TotalDiscount + Purchase.TaxAmount;
			Purchase.Details.ForEach(d =>
			{
				d.Amount = (d.Rate * d.Qty) - d.Discount + d.TaxAmount;
				d.PurchaseId = Purchase.Id;	
			});		
			var success = await _api.UpdatePurchaseAsync(Purchase); // call Update API
			if (!success)
			{
				ModelState.AddModelError("", "Unable to update sale.");
				return Page();
			}

			return RedirectToPage("Index");
		}
	}
}
