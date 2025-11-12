using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.POS.Web.Pages.Roles
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public RoleDto Role { get; set; } = new();

        public List<ScopeDto> AllScopes { get; set; } = new();

        [BindProperty(Name = "SelectedScopeIds")]
        public List<int> SelectedScopeIds { get; set; } = new();

        public async Task OnGetAsync()
        {
            AllScopes = (await _api.GetAllScopesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            Role.ScopeIds = SelectedScopeIds ?? new();
            var ok = await _api.CreateRoleAsync(Role);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to create role.");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
