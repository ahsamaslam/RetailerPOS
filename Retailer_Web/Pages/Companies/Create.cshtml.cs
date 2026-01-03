using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Companies
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api)
        {
            _api = api;
        }

        [BindProperty]
        public CompanyDto Company { get; set; } = new();

        public void OnGet()
        {
            Company.IsActive = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _api.CreateCompanyAsync(Company);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Unable to create company.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
