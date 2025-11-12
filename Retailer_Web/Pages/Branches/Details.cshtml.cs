using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Branches
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public BranchDto Branch { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var dto = await _api.GetBranchByIdAsync(id);
            if (dto != null) Branch = dto;
        }
    }
}
