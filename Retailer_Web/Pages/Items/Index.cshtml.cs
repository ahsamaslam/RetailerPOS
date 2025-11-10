using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Items;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IEnumerable<ItemDto> Items { get; set; } = Enumerable.Empty<ItemDto>();
    public IndexModel(IApiClient api) => _api = api;
    public async Task OnGetAsync() => Items = await _api.GetItemsAsync();
}
