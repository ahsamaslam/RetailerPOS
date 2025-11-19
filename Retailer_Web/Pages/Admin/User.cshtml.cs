using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Retailer.Web.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly HttpClient _client;

        public UsersModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7001/api/admin/"); // adjust API URL
        }

        [BindProperty]
        public CreateUserDto NewUser { get; set; }

        public List<UserViewModel> Users { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _client.GetFromJsonAsync<List<UserViewModel>>("users") ?? new List<UserViewModel>();
            AllRoles = await _client.GetFromJsonAsync<List<string>>("roles/names") ?? new List<string>();
            foreach (var user in Users)
            {
                user.Roles = await _client.GetFromJsonAsync<List<string>>($"users/{user.Id}/roles") ?? new List<string>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _client.PostAsJsonAsync("users", NewUser);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating user");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return;

            await _client.PostAsync($"users/{userId}/roles/{roleName}", null);
            await OnGetAsync();
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
    }

    public record CreateUserDto(string UserName, string Email, string Password);
}
