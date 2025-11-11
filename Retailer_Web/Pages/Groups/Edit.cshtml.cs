using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Groups;

public class EditModel : PageModel
{
    private readonly IApiClient _api;
    public EditModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemGroupViewModel Group { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var g = await _api.GetGroupAsync(id);
        if (g == null) return NotFound();
        Group = g;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var success = await _api.UpdateGroupAsync(Group);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Unable to update group.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
