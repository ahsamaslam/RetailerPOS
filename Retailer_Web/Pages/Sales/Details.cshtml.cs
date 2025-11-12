using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public SalesMasterDto Sale { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var dto = await _api.GetSaleByIdAsync(id);
            if (dto != null) Sale = dto;
        }
    }
}
