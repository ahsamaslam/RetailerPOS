using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Pages.SaleInvoiceSettings
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;

        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SaleInvoiceSettingDto Setting { get; set; } = new();

        public void OnGet()
        {
            // Initialize with default values if needed
            Setting = new SaleInvoiceSettingDto
            {
                PageSize = "A4",
                Orientation = "Portrait",
                ShowLogo = 0
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _api.CreateSalePrintSettingAsync(Setting);

            if (success)
            {
                TempData["SuccessMessage"] = "Invoice setting created successfully!";
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create invoice setting.");
            return Page();
        }
    }
}

