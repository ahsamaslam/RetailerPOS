using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Categories;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;
    public List<ItemCategoryViewModel> Categories { get; set; } = new();
    public async Task OnGetAsync() => Categories = await _api.GetCategoriesAsync();
}
