using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Scopes
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public ScopeDto Scope { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _api.GetScopeByIdAsync(id);
            if (dto == null) return NotFound();
            Scope = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var ok = await _api.UpdateScopeAsync(Scope);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to update scope.");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
