using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Vendors
{
    [Authorize]
    public class DetailsModel : BasePageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public VendorViewModel Vendor { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var vendor = await _api.GetVendorByIdAsync(id);
            if (vendor != null) Vendor = vendor;
        }
    }
}
