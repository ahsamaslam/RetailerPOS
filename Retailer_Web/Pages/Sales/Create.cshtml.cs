using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesViewModel Sale { get; set; } = new()
        {
            Details = new List<SalesDetailViewModel> { new SalesDetailViewModel() }
        };

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.CreateSalesAsync(Sale);
            return RedirectToPage("Index");
        }
    }
}
