using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.SubGroups;

public class EditModel : PageModel
{
    private readonly IApiClient _api;
    public EditModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemSubGroupViewModel SubGroup { get; set; } = new();

    public List<SelectListItem> GroupSelectList { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var sg = await _api.GetSubGroupAsync(id);
        if (sg == null) return NotFound();
        SubGroup = sg;

        await PopulateGroups();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateGroups();
            return Page();
        }

        var success = await _api.UpdateSubGroupAsync(SubGroup);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Unable to update subgroup.");
            await PopulateGroups();
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateGroups()
    {
        var groups = await _api.GetGroupsAsync();
        GroupSelectList = groups.Select(g => new SelectListItem(g.Name, g.Id.ToString())).ToList();
    }
}
