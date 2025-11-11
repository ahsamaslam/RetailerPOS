using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Groups;

public class DetailsModel : PageModel
{
    private readonly IApiClient _api;
    public DetailsModel(IApiClient api) => _api = api;

    public ItemGroupViewModel Group { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        var g = await _api.GetGroupAsync(id);
        if (g != null) Group = g;
    }
}
