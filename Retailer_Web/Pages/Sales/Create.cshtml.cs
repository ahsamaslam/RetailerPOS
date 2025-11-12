using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new()
        {
            Details = new List<SalesDetailDto> { new SalesDetailDto() }
        };

        public List<SelectListItem> ItemsList { get; set; } = new();
        public List<SelectListItem> CustomersList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Load dropdown data
            var items = await _api.GetItemsAsync();
            ItemsList = items.Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name }).ToList();

            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            if (id.HasValue)
            {
                Sale = await _api.GetSaleByIdAsync(id.Value);
                if (Sale == null) return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Recalculate totals
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            bool success;
            if (Sale.Id > 0)
                success = await _api.UpdateSaleAsync(Sale);
            else
                success = await _api.CreateSaleAsync(Sale);

            if (!success)
            {
                ModelState.AddModelError("", "Unable to save sale.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
