using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Scopes
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public ScopeDto Scope { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var ok = await _api.CreateScopeAsync(Scope);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to create scope.");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
