using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Vendors
{
    public class EditModel : BasePageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) : base(api) => _api = api;

        [BindProperty]
        public VendorViewModel Vendor { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var vendor = await _api.GetVendorByIdAsync(id);
            if (vendor != null) Vendor = vendor;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateVendorAsync(Vendor);
            return RedirectToPage("Index");
        }
    }
}
