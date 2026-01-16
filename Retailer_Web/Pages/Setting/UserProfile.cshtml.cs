using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using Retailer.Web.Pages.Admin;
using static System.Net.WebRequestMethods;
using System.Net;
using Retailer.POS.Web.ApiDTOs;
using System.Text.Json;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Retailer.Web.Pages.Setting
{
    [Authorize]
    public class UserProfileModel : BasePageModel
    {
        private readonly IApiClient _api;
        private IWebHostEnvironment env;
        private readonly HttpClient _httpFactory;
        private readonly HttpClient _http;
        public bool IsAdmin => User.IsInRole("admin");
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public UserProfileModel(IApiClient api, IWebHostEnvironment _env, IHttpClientFactory httpFactory)
        {
            env = _env;
            _api = api;
            _http = httpFactory.CreateClient("AuthApi");
        }

        [BindProperty]
        public IFormFile? LogoFile { get; set; } // For new file upload

        [BindProperty]
        public UserViewModel Input { get; set; } = new();
        public Guid companyID { get; set; } = new();
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadCurrentUserAsync();
                return Page();
            }
            catch (ApiUnauthorizedException)
            {
                return RedirectToLogin();
            }
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            ModelState.Remove("Input.picture");
            ModelState.Remove("Input.logoPath");

            if (!ModelState.IsValid)
            {
                await LoadCurrentUserAsync();
                return Page();
            }

            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(Input.Id ?? string.Empty), "UserId" },
                    { new StringContent(Input.UserName ?? string.Empty), "UserName" },
                    { new StringContent(Input.Email ?? string.Empty), "Email" }
                };

                if (LogoFile != null)
                {
                    var fileContent = new StreamContent(LogoFile.OpenReadStream());
                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(LogoFile.ContentType ?? "application/octet-stream");

                    content.Add(fileContent, "Picture", LogoFile.FileName);
                }

                var response = await _http.PutAsync("api/authuser/profile", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ApiUnauthorizedException();
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(error) ? "Unable to update profile" : error);
                    await LoadCurrentUserAsync();
                    return Page();
                }

                TempData["Success"] = "Profile updated.";
                await LoadCurrentUserAsync();
                return Page();
            }
            catch (ApiUnauthorizedException)
            {
                return RedirectToLogin();
            }
        }

        // ================= Password Change Handler =================
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                await CheckPasswordAsync(new UserPasswordDto { CurrentPassword = Input.oldPassword, NewPassword = Input.currentPasswordA, userID = Input.Id });
                await LoadCurrentUserAsync();
                return Page();
            }
            catch (ApiUnauthorizedException)
            {
                return RedirectToLogin();
            }
        }

        private async Task LoadCurrentUserAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                Input = new UserViewModel();
                return;
            }

            Input = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                picture = user.picture
            };

            LogoFile = null;
        }

        private async Task<UserDto?> GetCurrentUserAsync()
        {
            using var r = await _http.GetAsync("api/authuser/currentUser");
            if (r.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<UserDto>(_jsonOptions) ?? new UserDto();
        }

        private RedirectToPageResult RedirectToLogin() => RedirectToPage("/Login", new { returnUrl = Request.Path + Request.QueryString });

        private async Task<(bool value, string Message)> ChangePasswordAsync(UserPasswordDto dto)
        {
            using var r = await _http.PostAsJsonAsync("api/authuser/ChangePassword", dto, _jsonOptions);
            if (r.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            if (r.IsSuccessStatusCode) return (true, "Item Type created successfully");
            var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonOptions);
            string message = content != null && content.ContainsKey("message") ? content["message"] : await r.Content.ReadAsStringAsync();
            return (false, message);
        }

        private async Task<(bool value, string Message)> CheckPasswordAsync(UserPasswordDto dto)
        {
            using var r = await _http.PostAsJsonAsync("api/authuser/currentUserPassword", dto, _jsonOptions);
            if (r.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            if (r.IsSuccessStatusCode) return (true, "Item Type created successfully");
            var content = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonOptions);
            string message = content != null && content.ContainsKey("message") ? content["message"] : await r.Content.ReadAsStringAsync();
            return (false, message);
        }


    }
}
