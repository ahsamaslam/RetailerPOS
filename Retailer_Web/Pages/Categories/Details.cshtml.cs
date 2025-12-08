using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Categories;
public class DetailsModel : BasePageModel
{
    private readonly IApiClient _api;
    public DetailsModel(IApiClient api) : base(api) { _api = api; }
    public ItemCategoryViewModel Category { get; set; } = new();
    public async Task OnGetAsync(int id)
    {
        var c = await _api.GetCategoryAsync(id);
        if (c != null) Category = c;
    }
}
