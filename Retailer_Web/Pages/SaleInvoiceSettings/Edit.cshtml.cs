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
        public SaleInvoiceSettingDto Setting { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Setting = await _api.GetSalePrintSettingById(id);

            if (Setting == null) 
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _api.UpdateSalePrintSettingAsync(id, Setting);

            if (success)
            {
                TempData["SuccessMessage"] = "Invoice setting updated successfully!";
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to update invoice setting.");
            return Page();
        }
    }
}

