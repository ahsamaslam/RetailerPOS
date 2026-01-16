using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Pages.SaleInvoiceSettings
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

       
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;
        [BindProperty] 
        public List<SaleInvoiceSettingDto> Settings { get; set; } = new();

        public async Task OnGetAsync()
        {
            //GetSalePrintSettingList
            Settings  = await _api.GetSalePrintSettingList(); // fetch DTO from API
        }
    }

}
