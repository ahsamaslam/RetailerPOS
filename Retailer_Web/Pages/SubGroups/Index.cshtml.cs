using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.SubGroups;
[Authorize]
public class IndexModel : BasePageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;

    public List<ItemSubGroupViewModel> SubGroups { get; set; } = new();

    public async Task OnGetAsync()
    {
        SubGroups = await _api.GetSubGroupsAsync();
    }
}
