using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages.Admin
{
    public class RolesModel:PageModel
    {
        private readonly HttpClient _client;

        public RolesModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("AuthApi");
            //_client.BaseAddress = new Uri("https://localhost:7001/api/admin/");
        }

        // =========================
        // View Models
        // =========================
        public List<RoleViewModel> Roles { get; set; } = new();
        public List<PermissionViewModel> AllPermissions { get; set; } = new();
        [BindProperty]
        public string? SelectedRoleId { get; set; }

        public List<int> AssignedPermissionIds { get; set; } = new();

        // =========================
        // Bind Properties
        // =========================
        [BindProperty]
        public RoleEditDto EditRole { get; set; } = new();

        // =========================
        // GET
        // =========================
        public async Task OnGetAsync()
        {
            Roles = await _client.GetFromJsonAsync<List<RoleViewModel>>("api/admin/roles")
                    ?? new();

            AllPermissions = await _client.GetFromJsonAsync<List<PermissionViewModel>>("api/admin/permissions")
                    ?? new();
        }

        // =========================
        // CREATE ROLE
        // =========================
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(EditRole.Name))
            {
                ModelState.AddModelError(string.Empty, "Role name is required.");
                await OnGetAsync();
                return Page();
            }

            var res = await _client.PostAsJsonAsync("api/admin/roles", new CreateRoleDto(EditRole.Name));

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create role.");
                await OnGetAsync();
                return Page();
            }

            TempData["Success"] = "Role created successfully.";
            return RedirectToPage();
        }

        // =========================
        // EDIT ROLE
        // =========================
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (string.IsNullOrWhiteSpace(EditRole.Id))
                return BadRequest();

            // Identity role rename pattern:
            // delete + recreate OR custom endpoint
            // (assuming you add UpdateRole API later)
            ModelState.AddModelError(string.Empty, "Role rename API not implemented.");
            await OnGetAsync();
            return Page();
        }

        // =========================
        // DELETE ROLE
        // =========================
        public async Task<IActionResult> OnPostDeleteAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return BadRequest();

            var res = await _client.DeleteAsync($"api/admin/roles/{roleId}");

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete role.";
                return RedirectToPage();
            }

            TempData["Success"] = "Role deleted.";
            return RedirectToPage();
        }

        // =========================
        // ASSIGN PERMISSIONS TO ROLE
        // =========================
        public async Task<IActionResult> OnPostAssignPermissionsAsync(
            string roleId,
            List<int> selectedPermissions)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return BadRequest();

            selectedPermissions ??= new();

            // Remove all permissions first (if your API supports it)
            // Then re-assign selected permissions
            foreach (var pid in selectedPermissions)
            {
                await _client.PostAsync(
                    $"api/admin/roles/{roleId}/permissions/{pid}",
                    null
                );
            }

            TempData["Success"] = "Permissions updated.";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostLoadPermissionsAsync(string roleId, string roleName)
        {
            SelectedRoleId = roleId;
            ViewData["SelectedRoleName"] = roleName;

            // Load roles + permissions for page
            await OnGetAsync();
            var perms = await _client
                .GetFromJsonAsync<List<PermissionViewModel>>($"api/admin/roles/{roleId}/permissions")
                ?? new();

            AssignedPermissionIds = perms.Select(p => p.Id).ToList();

            // Tell Razor to reopen modal
            ViewData["OpenPermissionsModal"] = true;

            return Page();
        }

    }

    // =========================
    // Supporting DTOs
    // =========================
    public class RoleViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; }
    }

    public class RoleEditDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = "";
    }

    public record CreateRoleDto(string Name);
}
