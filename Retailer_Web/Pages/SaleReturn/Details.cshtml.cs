using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.Web.Pages.SaleReturn
{
    [Authorize]
    public class DetailsModel : BasePageModel
    {
        private readonly IApiClient _api;

        public DetailsModel(IApiClient api) => _api = api;

        public SalesMasterReturnDto SaleReturn { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _api.GetSaleReturnByIdAsync(id);
            if (dto is null)
            {
                return NotFound();
            }

            SaleReturn = dto;
            return Page();
        }
    }
}