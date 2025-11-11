using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.ItemType;
public class EditModel : PageModel
{
    private readonly IApiClient _api;
    public EditModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemTypeViewModel ItemType { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        var c = await _api.GetItemTypeAsync(id);
        if (c != null) ItemType = c;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.UpdateItemTypeAsync(ItemType);
        return RedirectToPage("Index");
    }
}
