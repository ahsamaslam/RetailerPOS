using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Companies
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) { _api = api; }

        public IEnumerable<CompanyDto> Companies { get; set; }

        public async Task OnGetAsync()
        {
            Companies = await _api.GetCompanyAsync();
        }
    }
}
