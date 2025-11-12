using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new SalesMasterDto
        {
            Details = new List<SalesDetailDto> { new SalesDetailDto() }
        };

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // compute subtotals/amounts server-side or expect client to set Amount
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            var success = await _api.CreateSaleAsync(Sale);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create sale.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
