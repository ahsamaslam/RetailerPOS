using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.SubGroups;

public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;

    public List<ItemSubGroupViewModel> SubGroups { get; set; } = new();

    public async Task OnGetAsync()
    {
        SubGroups = await _api.GetSubGroupsAsync();
    }
}
