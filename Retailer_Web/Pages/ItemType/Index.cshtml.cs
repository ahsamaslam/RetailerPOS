using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.ItemType;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;
    public List<ItemTypeViewModel> ItemType { get; set; } = new();
    public async Task OnGetAsync() => ItemType = await _api.GetItemTypeAsync();
}
