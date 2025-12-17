using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Vendors
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;

        public List<VendorViewModel> Vendors { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vendors = await _api.GetVendorsAsync();
        }
    }
}
