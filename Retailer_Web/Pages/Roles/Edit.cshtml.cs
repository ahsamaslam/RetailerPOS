using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Roles
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public RoleDto Role { get; set; } = new();

        public List<ScopeDto> AllScopes { get; set; } = new();

        [BindProperty(Name = "SelectedScopeIds")]
        public List<int> SelectedScopeIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            AllScopes = (await _api.GetAllScopesAsync()).ToList();
            var dto = await _api.GetRoleByIdAsync(id);
            if (dto == null) return NotFound();
            Role = dto;
            SelectedScopeIds = dto.ScopeIds ?? new();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Role.ScopeIds = SelectedScopeIds ?? new();
            var ok = await _api.UpdateRoleAsync(Role);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to update role.");
                AllScopes = (await _api.GetAllScopesAsync()).ToList();
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
