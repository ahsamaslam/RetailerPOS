using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.ItemType;
public class DetailsModel : PageModel
{
    private readonly IApiClient _api;
    public DetailsModel(IApiClient api) => _api = api;
    public ItemTypeViewModel ItemType { get; set; } = new();
    public async Task OnGetAsync(int id)
    {
        var c = await _api.GetItemTypeAsync(id);
        if (c != null) ItemType = c;
    }
}
