using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Purchases
{
    [Authorize]
    public class EditModel : BasePageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public PurchaseMasterDto Purchase { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
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
