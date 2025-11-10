using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Sales
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesViewModel Sale { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var sale = await _api.GetSaleByIdAsync(id);
            if (sale != null) Sale = sale;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateSaleAsync(Sale);
            return RedirectToPage("Index");
        }
    }
}
