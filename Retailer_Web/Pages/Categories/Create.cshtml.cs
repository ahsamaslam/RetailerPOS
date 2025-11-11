using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Categories;
public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreateModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemCategoryViewModel Category { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.CreateCategoryAsync(Category);
        return RedirectToPage("Index");
    }
}
