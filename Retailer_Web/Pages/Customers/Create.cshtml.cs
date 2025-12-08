using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Customers
{
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) : base(api) { _api = api; }

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateCustomerAsync(Customer);
            return RedirectToPage("Index");
        }
    }
}
