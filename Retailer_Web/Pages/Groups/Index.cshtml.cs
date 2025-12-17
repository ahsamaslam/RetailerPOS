using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Groups;
[Authorize]
public class IndexModel : BasePageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) { _api = api; }

    public List<ItemGroupViewModel> Groups { get; set; } = new();

    public async Task OnGetAsync()
    {
        Groups = await _api.GetGroupsAsync();
    }
}
