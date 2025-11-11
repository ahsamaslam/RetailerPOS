using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Groups;

public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;

    public List<ItemGroupViewModel> Groups { get; set; } = new();

    public async Task OnGetAsync()
    {
        Groups = await _api.GetGroupsAsync();
    }
}
