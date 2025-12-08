using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.ItemType;
public class IndexModel : BasePageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) : base(api) { _api = api; }
    public List<ItemTypeViewModel> ItemType { get; set; } = new();
    public async Task OnGetAsync() => ItemType = await _api.GetItemTypeAsync();
}
