using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Retailer.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Admin
{

    public class PermissionsModel : BasePageModel
    {
        private readonly HttpClient _client;
        private readonly IApiClient _api;

        public PermissionsModel(IHttpClientFactory factory,IApiClient api):base(api)
        {
            _client = factory.CreateClient("AuthApi");
            _api = api;
        }

        public List<PermissionViewModel> Permissions { get; set; } = new();

        [BindProperty]
        public PermissionDto EditPermission { get; set; } = new();

        public async Task OnGetAsync()
        {
            Permissions = await _client
                .GetFromJsonAsync<List<PermissionViewModel>>("api/admin/permissions")
                ?? new();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var res = await _client.PostAsJsonAsync("api/admin/permissions", EditPermission);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create permission.");
                await OnGetAsync();
                return Page();
            }

            TempData["Success"] = "Permission created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var res = await _client.PutAsJsonAsync(
                $"api/admin/permissions/{EditPermission.Id}", EditPermission);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update permission.");
                await OnGetAsync();
                return Page();
            }

            TempData["Success"] = "Permission updated.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int permissionId)
        {
            var res = await _client.DeleteAsync($"api/admin/permissions/{permissionId}");

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete permission.";
                return RedirectToPage();
            }

            TempData["Success"] = "Permission deleted.";
            return RedirectToPage();
        }
    }
}
