using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;

        public List<SalesMasterDto> Sales { get; set; } = new();

        public async Task OnGetAsync()
        {
            Sales = (await _api.GetSalesAsync()).ToList();
        }
    }
}
