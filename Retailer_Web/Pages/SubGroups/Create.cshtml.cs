using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.SubGroups;

public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreateModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemSubGroupViewModel SubGroup { get; set; } = new();

    public List<SelectListItem> GroupSelectList { get; set; } = new();

    public async Task OnGetAsync()
    {
        var groups = await _api.GetGroupsAsync();
        GroupSelectList = groups.Select(g => new SelectListItem(g.Name, g.Id.ToString())).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateGroups();
            return Page();
        }

        var success = await _api.CreateSubGroupAsync(SubGroup);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Unable to create subgroup.");
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
