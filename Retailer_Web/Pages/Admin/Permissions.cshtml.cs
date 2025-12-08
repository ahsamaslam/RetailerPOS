using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;
using System.Net.Http.Json;

namespace Admin.Pages.Admin
{
    public class PermissionsModel : BasePageModel
    {
        private readonly HttpClient _client;
        private readonly IApiClient _api;
        public PermissionsModel(IHttpClientFactory factory,IApiClient api) : base(api)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7001/api/admin/");
            _api = api;
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
