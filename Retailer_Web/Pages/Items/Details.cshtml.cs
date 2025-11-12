using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Items;

public class DetailsModel : PageModel
{
    private readonly IApiClient _api;

    public DetailsModel(IApiClient api) => _api = api;

    public ItemDto Item { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await _api.GetItemAsync(id) ?? new ItemDto();
        if (Item.Id == 0) return NotFound();
        return Page();
    }
}
