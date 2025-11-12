using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;

        public List<RoleListViewModel> Roles { get; set; } = new();

        public async Task OnGetAsync()
        {
            var roles = (await _api.GetAllRolesAsync()).ToList();
            var scopes = (await _api.GetAllScopesAsync()).ToList();

            Roles = roles.Select(r => new RoleListViewModel
            {
                Id = r.Id,
                RoleName = r.RoleName,
                ScopeNames = string.Join(", ", scopes.Where(s => r.ScopeIds.Contains(s.Id)).Select(s => s.ScopeName))
            }).ToList();
        }

        public class RoleListViewModel
        {
            public int Id { get; set; }
            public string RoleName { get; set; } = "";
            public string? ScopeNames { get; set; }
        }
    }
}
