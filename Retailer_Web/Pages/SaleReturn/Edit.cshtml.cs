using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.SaleReturn
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesMasterReturnDto Sale { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Sale = await _api.GetSaleReturnByIdAsync(id); // fetch DTO from API
            if (Sale == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // recompute totals server-side
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            var success = await _api.UpdateSaleReturnAsync(Sale); // call Update API
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update sale.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
