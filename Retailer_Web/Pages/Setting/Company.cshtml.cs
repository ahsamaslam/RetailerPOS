using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.Web.Setting
{
    public class CompanyModel : BasePageModel
    {
        private readonly IApiClient _api;
        private IWebHostEnvironment env;
        public bool IsAdmin => User.IsInRole("admin");
        public CompanyModel(IApiClient api, IWebHostEnvironment _env) : base(api)
        {
            env = _env;
            _api = api;
        }
        [BindProperty]
        public IFormFile? LogoFile { get; set; } // For new file upload
        public string? logoPath { get; set; } // For new file upload

        [BindProperty]
        public CompanyViewModel Input { get; set; } = new();
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
            var folderPath = Path.Combine(env.WebRootPath, "uploads", "CompanyLogo");
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

        public async Task OnGetAsync()
        { 
            var company = await _api.GetUserCompanyAsync();
           
            
            if (company != null)
            {
                companyID =  company.Id;
                Input = new CompanyViewModel
                {       Id =  company.Id,
                    Name = company.Name,
                    Address = company.Address,
                    ContactPhone = company.ContactPhone,
                    ContactEmail = company.ContactEmail,
                    NTN = company.NTN,
                    CNIC = company.CNIC ,
                    STRN = company.STRN ,
                     ContactPerson = company.ContactPerson  ,
                     ShortName = company.ShortName  ,
                     fbrToken = company.fbrToken    ,
                      pralToken = company.pralToken,
                      fbrActive = company.fbrActive ,
                      Province =  company.Province,
                      logoPath = company.logoPath,
                      CompanyType = company.CompanyType,   
                      isEd =  company.isEd , 
                      isFed = company.isFed,
                      isGst = company.isGst,
                      gstVal =  company.gstVal,
                      fedVal=  company.fedVal,
                       edVal = company.edVal
                      
                };
                if (company.logoPath != null)
                {
                    LogoFile = GetFormFileFromPath(company.logoPath);
                    logoPath = company.logoPath;
                }
                 
            }           
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

                var folderPath = Path.Combine(env.WebRootPath, "uploads", "CompanyLogo");
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            //if(logoPath!=null)

            Input.logoPath =await SaveLogoAsync(LogoFile);
              (bool Success, string Message) = await _api.UpdateCompanyAsync(Input);

            if (!Success)
            {
                ModelState.AddModelError(string.Empty, Message);
                return Page();
            }


            return Page();
        }
    }
}
