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
        public async Task<string> SaveLogoAsync(IFormFile? logo)
        {
            if (logo == null || logo.Length == 0)
                return null;

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logo.FileName)}";
            try
            {


                // Save path in wwwroot/uploads

                var folderPath = Path.Combine(env.WebRootPath, "uploads", "UserLogo");
                Directory.CreateDirectory(folderPath);
                // Ensure the folder exists

                var savePath = Path.Combine(folderPath, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                // Save the file
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }
            }
            catch (Exception exx)
            {

            }
            // Return the relative URL to use in img src
            return $"/uploads/CompanyLogo/{fileName}";
        }
        public async Task<IFormFile?> GetIFormFileFromUrlAsync(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var contentStream = await response.Content.ReadAsStreamAsync();
            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            // Derive filename from URL (or set your own)
            var fileName = Path.GetFileName(new Uri(url).AbsolutePath);

            // Create the IFormFile from memory
            var formFile = new FormFile(
                baseStream: new MemoryStream(contentBytes),
                baseStreamOffset: 0,
                length: contentBytes.Length,
                name: "file",
                fileName: fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream"
            };

            return formFile;
        }
        public async Task OnGetAsync()
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                
               Input = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    picture = user.picture
              };
                if (user.picture != null)
                {
                    LogoFile = await GetIFormFileFromUrlAsync(user.picture); 
                }
            }
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(Input.Id), "UserId" },
                { new StringContent(Input.UserName), "UserName" },
                { new StringContent(Input.Email ?? ""), "Email" }
            };

            if (LogoFile != null)
            {
                var fileContent = new StreamContent(LogoFile.OpenReadStream());
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(LogoFile.ContentType);

                content.Add(fileContent, "Picture", LogoFile.FileName);
            }

            var response = await _http.PutAsync("api/authuser/profile", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return Page();
        }
        // ================= Password Change Handler =================
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await CheckPasswordAsync( new UserPasswordDto { CurrentPassword  =  Input.oldPassword , NewPassword =  Input.currentPasswordA, userID  = Input.Id });
            return Page();
        }
        private async Task<UserDto?> GetCurrentUserAsync()
        {
            using var r = await _http.GetAsync("api/authuser/currentUser");
            if (r.StatusCode == HttpStatusCode.Unauthorized) throw new ApiUnauthorizedException();
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<UserDto>(_jsonOptions) ?? new UserDto();
        }

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
