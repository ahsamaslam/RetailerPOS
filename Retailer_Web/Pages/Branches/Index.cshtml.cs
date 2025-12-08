using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Branches
{
    public class IndexModel : BasePageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) : base(api) { _api = api; }

        public List<BranchDto> Branches { get; set; } = new();

        public async Task OnGetAsync()
        {
            Branches = (await _api.GetAllBranchesAsync()).ToList();
        }
    }
}
