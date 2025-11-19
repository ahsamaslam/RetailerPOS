using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Admin.Pages.Admin
{
    public class PermissionsModel : PageModel
    {
        private readonly HttpClient _client;

        public PermissionsModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7001/api/admin/");
        }

        [BindProperty]
        public CreatePermissionDto NewPermission { get; set; }

        public List<PermissionViewModel> Permissions { get; set; } = new();

        public async Task OnGetAsync()
        {
            Permissions = await _client.GetFromJsonAsync<List<PermissionViewModel>>("permissions") ?? new List<PermissionViewModel>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _client.PostAsJsonAsync("permissions", NewPermission);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating permission");
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }
    }

    public class PermissionViewModel { public int Id { get; set; } public string Name { get; set; } = ""; public string? Description { get; set; } }
    public record CreatePermissionDto(string Name, string? Description);
}
