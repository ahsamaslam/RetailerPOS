using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Scopes
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;

        public List<ScopeDto> Scopes { get; set; } = new();

        public async Task OnGetAsync()
        {
            Scopes = (await _api.GetAllScopesAsync()).ToList();
        }
    }
}
