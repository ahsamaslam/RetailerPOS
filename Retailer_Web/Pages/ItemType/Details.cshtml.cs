using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.ItemType;
[Authorize]
public class DetailsModel : BasePageModel
{
    private readonly IApiClient _api;
    public DetailsModel(IApiClient api) { _api = api; }
    public ItemTypeViewModel ItemType { get; set; } = new();
    public async Task OnGetAsync(int id)
    {
        var c = await _api.GetItemTypeAsync(id);
        if (c != null) ItemType = c;
    }
}
