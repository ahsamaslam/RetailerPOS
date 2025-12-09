using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using System.Net.Http.Json;

namespace Retailer.Web.Pages.Admin
{
    public class UsersModel : BasePageModel
    {
        private readonly HttpClient _client;
        private readonly IApiClient _api;
        public UsersModel(IHttpClientFactory factory, IApiClient api): base(api)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7001/api/admin/"); // adjust API URL as needed
            _api = api;
        }

        [BindProperty]
        public CreateUserDto NewUser { get; set; } = new();

        public List<UserViewModel> Users { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _client.GetFromJsonAsync<List<UserViewModel>>("users") ?? new List<UserViewModel>();
            AllRoles = await _client.GetFromJsonAsync<List<string>>("roles/names") ?? new List<string>();

            // populate each user's roles (API call per user)
            foreach (var user in Users)
            {
                user.Roles = await _client.GetFromJsonAsync<List<string>>($"users/{user.Id}/roles") ?? new List<string>();
            }
        }

        /// <summary>
        /// Handler for creating a user. Form should post to the page (default POST -> OnPostAsync).
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var response = await _client.PostAsJsonAsync("users", NewUser);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating user");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage(); // refresh list after creation
        }

        /// <summary>
        /// Handler for assigning role. The form should post with asp-page-handler="AssignRole".
        /// Expects fields named "UserId" and "RoleName".
        /// </summary>
        public async Task<IActionResult> OnPostAssignRoleAsync(string UserId, string RoleName)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(RoleName))
            {
                // nothing to do
                return RedirectToPage();
            }

            var response = await _client.PostAsync($"users/{UserId}/roles/{RoleName}", null);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error assigning role");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage(); // reload to show updated roles
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string oldPassword { get; set; } = "";
        public string currentPasswordA { get; set; } = "";
        public string currentPasswordB { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public IFormFile? picture { get; set; }
        public string? logoPath { get; set; }
    }

    // Use a simple class for model-binding with BindProperty
    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
