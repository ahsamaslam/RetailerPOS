using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Categories;
[Authorize]
public class EditModel : BasePageModel
{
    private readonly IApiClient _api;
    public EditModel(IApiClient api) { _api = api; }

    [BindProperty]
    public ItemCategoryViewModel Category { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        var c = await _api.GetCategoryAsync(id);
        if (c != null) Category = c;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.UpdateCategoryAsync(Category);
        return RedirectToPage("Index");
    }
}
