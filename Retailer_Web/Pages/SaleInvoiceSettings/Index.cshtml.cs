using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Pages.SaleInvoiceSettings
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;

        public IndexModel(IApiClient api) => _api = api;

        [BindProperty] 
        public List<SaleInvoiceSettingDto> Settings { get; set; } = new();

        public async Task OnGetAsync()
        {
            Settings = await _api.GetSalePrintSettingList() ?? new List<SaleInvoiceSettingDto>();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _api.DeleteSalePrintSettingAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Invoice setting deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete invoice setting.";
            }

            return RedirectToPage();
        }
    }
}

