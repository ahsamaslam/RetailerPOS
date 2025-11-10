using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Sales
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public SalesViewModel Sale { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var sale = await _api.GetSaleByIdAsync(id);
            if (sale != null) Sale = sale;
        }
    }
}
