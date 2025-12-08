using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Items;
public class IndexModel : BasePageModel
{
    private readonly IApiClient _api;

    public IndexModel(IApiClient api) : base(api) { _api = api; }

    public IList<ItemDto> Items { get; set; } = new List<ItemDto>();

    public async Task OnGetAsync()
    {
        Items = (await _api.GetItemsAsync()).ToList();
    }
}
