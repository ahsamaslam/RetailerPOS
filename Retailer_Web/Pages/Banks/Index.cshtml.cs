using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.Banks
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) { _api = api; }

        public List<BanksViewModel> Banks { get; set; } = new();

        public async Task OnGetAsync()
        {
            Banks = await _api.GetBanksAsync();
        }
    }
}
