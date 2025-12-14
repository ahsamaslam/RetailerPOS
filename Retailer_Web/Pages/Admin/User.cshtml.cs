using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.Admin
{
    public class UsersModel : BasePageModel
    {
        private readonly HttpClient _client;
        private readonly IApiClient _api;
        private readonly ILogger<UsersModel> _logger;

        public UsersModel(IHttpClientFactory factory, IApiClient api, ILogger<UsersModel> logger) : base(api)
        {
            _client = factory.CreateClient("AuthApi");
            // adjust API URL as needed (or use IConfiguration to read it)
            //_client.BaseAddress = new Uri("https://localhost:7001/api/admin/");
            _api = api;
            _logger = logger;
        }

        // ----------------- Bind properties for forms -----------------
        [BindProperty]
        public CreateUserDto NewUser { get; set; } = new();

        [BindProperty]
        public EditUserDto EditUser { get; set; } = new();

        // Roles posted from roles modal (SelectedRoles[] checkboxes)
        [BindProperty]
        public string[] SelectedRoles { get; set; } = Array.Empty<string>();

        // Hidden UserId for block/delete/unblock forms
        [BindProperty]
        public string? UserId { get; set; }

        // UI model data
        public List<UserViewModel> Users { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();

        // pagination support (optional)
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1

;        // ----------------- Page handlers -----------------

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            Page = page;

            try
            {
                // get users and role names
                Users = await _client.GetFromJsonAsync<List<UserViewModel>>("api/admin/users") ?? new List<UserViewModel>();
                AllRoles = await _client.GetFromJsonAsync<List<string>>("api/admin/roles/names") ?? new List<string>();

                // populate roles per user (API call per user) - this keeps backward compatibility
                foreach (var user in Users)
                {
                    try
                    {
                        user.Roles = await _client.GetFromJsonAsync<List<string>>($"api/admin/users/{user.Id}/roles") ?? new List<string>();
                    }
                    catch (HttpRequestException rex)
                    {
                        _logger.LogWarning(rex, "Failed to retrieve roles for user {UserId}", user.Id);
                        user.Roles = new List<string>();
                    }
                }

                // optional simple paging if API supports - for now this is client-side values
                TotalPages = 1;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                // API returned 401 — redirect to login
                return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users page");
                TempData["Error"] = "Unable to load users at this time.";
            }

            return Page();
        }

        /// <summary>
        /// Create user (handler name matches the modal action: ?handler=Create).
        /// </summary>
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                var resp = await _client.PostAsJsonAsync("api/admin/users", EditUser);
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    TempData["Error"] = $"Create failed: {err}";
                    await OnGetAsync();
                    return Page();
                }

                TempData["Success"] = "User created successfully";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                TempData["Error"] = "Unexpected error creating user";
                await OnGetAsync();
                return Page();
            }
        }

        /// <summary>
        /// Edit user (handler: ?handler=Edit)
        /// </summary>
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (EditUser == null || string.IsNullOrEmpty(EditUser.Id))
            {
                ModelState.AddModelError(string.Empty, "Invalid user data.");
                await OnGetAsync();
                return Page();
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(EditUser.UserName) || string.IsNullOrWhiteSpace(EditUser.Email))
            {
                ModelState.AddModelError(string.Empty, "Username and Email are required.");
                await OnGetAsync();
                return Page();
            }

            try
            {
                // Prepare payload - password is optional (leave null/empty to keep existing password)
                var payload = new
                {
                    UserName = EditUser.UserName,
                    Email = EditUser.Email,
                    Password = string.IsNullOrWhiteSpace(EditUser.Password) ? null : EditUser.Password,
                    InitialRole = EditUser.InitialRole // optional
                };

                var resp = await _client.PutAsJsonAsync($"api/admin/users/{EditUser.Id}", payload);
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    ModelState.AddModelError(string.Empty, $"Update failed: {err}");
                    await OnGetAsync();
                    return Page();
                }

                TempData["Success"] = "User updated successfully";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing user {UserId}", EditUser?.Id);
                ModelState.AddModelError(string.Empty, "Unexpected error updating user");
                await OnGetAsync();
                return Page();
            }
        }

        /// <summary>
        /// Assign multiple roles to a user (handler: ?handler=AssignRoles)
        /// </summary>
        public async Task<IActionResult> OnPostAssignRolesAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                ModelState.AddModelError(string.Empty, "No user specified.");
                await OnGetAsync();
                return Page();
            }

            try
            {
                // API contract assumed: POST users/{id}/roles with JSON array body
                var resp = await _client.PostAsJsonAsync($"api/admin/users/{UserId}/roles", SelectedRoles ?? Array.Empty<string>());
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    ModelState.AddModelError(string.Empty, $"Assigning roles failed: {err}");
                    await OnGetAsync();
                    return Page();
                }

                TempData["Success"] = "Roles updated";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning roles to user {UserId}", UserId);
                ModelState.AddModelError(string.Empty, "Unexpected error assigning roles");
                await OnGetAsync();
                return Page();
            }
        }

        /// <summary>
        /// Block a user (handler: ?handler=Block)
        /// </summary>
        public async Task<IActionResult> OnPostBlockAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToPage();
            }

            try
            {
                var resp = await _client.PostAsync($"api/admin/users/{UserId}/block", null);
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    TempData["Error"] = $"Block failed: {err}";
                    return RedirectToPage();
                }

                TempData["Success"] = "User blocked";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user {UserId}", UserId);
                TempData["Error"] = "Unexpected error blocking user";
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Unblock a user (handler: ?handler=Unblock)
        /// </summary>
        public async Task<IActionResult> OnPostUnblockAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToPage();
            }

            try
            {
                var resp = await _client.PostAsync($"api/admin/users/{UserId}/unblock", null);
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    TempData["Error"] = $"Unblock failed: {err}";
                    return RedirectToPage();
                }

                TempData["Success"] = "User unblocked";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking user {UserId}", UserId);
                TempData["Error"] = "Unexpected error unblocking user";
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Delete user (handler: ?handler=Delete). Confirmed via client.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToPage();
            }

            try
            {
                var resp = await _client.DeleteAsync($"api/admin/users/{UserId}");
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await TryReadErrorMessage(resp);
                    TempData["Error"] = $"Delete failed: {err}";
                    return RedirectToPage();
                }

                TempData["Success"] = "User deleted";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", UserId);
                TempData["Error"] = "Unexpected error deleting user";
            }

            return RedirectToPage();
        }

        // ----------------- Helper methods -----------------
        private static async Task<string> TryReadErrorMessage(HttpResponseMessage resp)
        {
            try
            {
                // try read { message: "..." } JSON
                var dict = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (dict != null && dict.TryGetValue("message", out var msg)) return msg;
            }
            catch { /* ignore */ }

            try
            {
                return await resp.Content.ReadAsStringAsync();
            }
            catch { return "Unknown error"; }
        }
    }

    // --------- VIEW MODELS / DTOs used by the page (adjust if you have real DTOs) ----------
    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public bool IsBlocked { get; set; } = false;
        public string? AvatarUrl { get; set; }
        public string oldPassword { get; set; } = "";
        public string currentPasswordA { get; set; } = ""; 
        public string currentPasswordB { get; set; } = "";
        public IFormFile? picture { get; set; }
        public string? logoPath { get; set; }
    }

    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        // optional: initial role to assign at creation
        public string? InitialRole { get; set; }
    }
    public class EditUserDto
    {
        public string? Id { get; set; }   //

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }
        public string? InitialRole { get; set; }
    }

    // Generic API result helper used elsewhere (if you have your own, remove this)
    public class ApiResult<T>
    {
        public bool Success { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        public ApiResult(bool success, T? data = default, string? err = null)
        {
            Success = success;
            Data = data;
            ErrorMessage = err;
        }
    }

    public class ApiResult
    {
        public bool Success { get; }
        public string? ErrorMessage { get; }
        public ApiResult(bool success, string? err = null)
        {
            Success = success;
            ErrorMessage = err;
        }
    }
}
