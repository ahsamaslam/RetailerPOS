using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var customer = await _api.GetCustomerByIdAsync(id);
            if (customer != null) Customer = customer;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateCustomerAsync(Customer);
            return RedirectToPage("Index");
        }
    }
}
