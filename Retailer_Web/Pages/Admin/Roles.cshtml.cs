using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Retailer.Web.Pages.Admin
{
    public class RolesModel : PageModel
    {
        private readonly HttpClient _client;

        public RolesModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7001/api/admin/");
        }

        [BindProperty]
        public CreateRoleDto NewRole { get; set; }

        public List<RoleViewModel> Roles { get; set; } = new();
        public List<PermissionViewModel> AllPermissions { get; set; } = new();

        public async Task OnGetAsync()
        {
            Roles = await _client.GetFromJsonAsync<List<RoleViewModel>>("roles") ?? new List<RoleViewModel>();
            AllPermissions = await _client.GetFromJsonAsync<List<PermissionViewModel>>("permissions") ?? new List<PermissionViewModel>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _client.PostAsJsonAsync("roles", NewRole);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating role");
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task AssignPermission(string roleId, int permissionId)
        {
            await _client.PostAsync($"roles/{roleId}/permissions/{permissionId}", null);
            await OnGetAsync();
        }
    }

    public class RoleViewModel { public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
    public class PermissionViewModel { public int Id { get; set; } public string Name { get; set; } = ""; }
    public record CreateRoleDto(string Name);
}
