using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.DTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Items.IndexModel;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;

    public IndexModel(IApiClient api) => _api = api;

    public IList<ItemDto> Items { get; set; } = new List<ItemDto>();

    public async Task OnGetAsync()
    {
        Items = (await _api.GetItemsAsync()).ToList();
    }
}
