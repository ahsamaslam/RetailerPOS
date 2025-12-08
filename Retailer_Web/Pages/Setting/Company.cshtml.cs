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
        public bool IsAdmin => User.IsInRole("admin");
        public CompanyModel(IApiClient api) : base(api) => _api = api;
        [BindProperty]
        public CompanyViewModel Input { get; set; } = new();

        public async Task OnGetAsync()
        { 
            var company = await _api.GetUserCompanyAsync();
            if (company != null)
            {
                Input = new CompanyViewModel
                {
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
                      Province =  company.Province
                };
            }           
        }
    }
}
