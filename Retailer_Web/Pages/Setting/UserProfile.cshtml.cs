using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;
using Retailer.Web.Pages.Admin;

namespace Retailer.Web.Pages.Setting
{
    public class UserProfileModel : BasePageModel
    {
        private readonly IApiClient _api;
        private IWebHostEnvironment env;
        public bool IsAdmin => User.IsInRole("admin");
        public UserProfileModel(IApiClient api, IWebHostEnvironment _env) : base(api)
        {
            env = _env;
            _api = api;
        }
        [BindProperty]
        public IFormFile? LogoFile { get; set; } // For new file upload

        [BindProperty]
        public UserViewModel Input { get; set; } = new();
        public Guid companyID { get; set; } = new();
        public IFormFile? GetFormFileFromPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            // Remove leading slashes
            relativePath = relativePath.TrimStart('/');

            // Remove "uploads/CompanyLogo/" prefix if present
            var prefix = "uploads/CompanyLogo/";
            if (relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(prefix.Length);
            }

            // Combine with wwwroot/uploads/CompanyLogo
            var folderPath = Path.Combine(env.WebRootPath, "uploads", "UserLogo");
            var filePath = Path.Combine(folderPath, relativePath); 
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath); 
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read); 
            IFormFile formFile = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream" // or detect MIME type dynamically
            };

            return formFile;
        }
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

        public async Task OnGetAsync()
        {
            var user = await _api.GetCurrentUserAsync();

            if (user != null)
            {
                
               Input = new UserViewModel
                {
                Id = user.Id,
               UserName = user.UserName,
                   Email = user.Email
                //    Address = company.Address,
                //    ContactPhone = company.ContactPhone,
                //    ContactEmail = company.ContactEmail,
                //    NTN = company.NTN,
                //    CNIC = company.CNIC,
                //    STRN = company.STRN,
                //    ContactPerson = company.ContactPerson,
                //    ShortName = company.ShortName,
                //    fbrToken = company.fbrToken,
                //    pralToken = company.pralToken,
                //    fbrActive = company.fbrActive,
                //    Province = company.Province,
                //    logoPath = company.logoPath,
              };
              if (user.picture != null)
                 LogoFile = GetFormFileFromPath(user.picture);
            }
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            if (!ModelState.IsValid) return Page();
            Input.logoPath = await SaveLogoAsync(LogoFile);
            return Page();
            //var user = await _api.GetCurrentUserAsync();
        }
        // ================= Password Change Handler =================
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _api.CheckPasswordAsync( new UserPasswordDto { CurrentPassword  =  Input.oldPassword , NewPassword =  Input.currentPasswordA, userID  = Input.Id });
            return Page();
        }
          

         
    }
}
