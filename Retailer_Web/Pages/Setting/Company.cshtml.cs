using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.Web.Setting
{
    [Authorize]
    public class CompanyModel : BasePageModel
    {
        private readonly IApiClient _api;
        private IWebHostEnvironment env;
        public bool IsAdmin => User.IsInRole("admin");
        public CompanyModel(IApiClient api, IWebHostEnvironment _env)
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
                    LogoFile =await GetIFormFileFromUrlAsync(company.logoPath);
                    logoPath = company.logoPath;
                }



            }           
        }
    
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            //if(logoPath!=null)

          //  Input.logoPath =c SaveLogoAsync(LogoFile);
            (bool Success, string Message) = (false,null); 
            //await _api.UpdateCompanyAsync(Input);

            if (!Success)
            {
                ModelState.AddModelError(string.Empty, Message);
                return Page();
            }


            return Page();
        }
    }
}
