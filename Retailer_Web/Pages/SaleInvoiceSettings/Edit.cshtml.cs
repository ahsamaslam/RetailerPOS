using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Pages.SaleInvoiceSettings
{
    public class EditModel : PageModel
    {

        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;
        [BindProperty] 
        public SaleInvoiceSettingDto Setting { get; set; } 
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Setting = await _api.GetSalePrintSetting(1); // fetch DTO from API 
             
           
            if (Setting == null) return NotFound();
            return Page();
        }
    }
}
