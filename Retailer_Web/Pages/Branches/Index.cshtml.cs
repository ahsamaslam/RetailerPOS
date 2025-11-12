using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.POS.Web.Pages.Branches
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;

        public List<BranchDto> Branches { get; set; } = new();

        public async Task OnGetAsync()
        {
            Branches = (await _api.GetAllBranchesAsync()).ToList();
        }
    }
}
