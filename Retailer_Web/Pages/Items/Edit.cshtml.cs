using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.DTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Items;

public class EditModel : PageModel
{
    private readonly IApiClient _api;

    public EditModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemDto Input { get; set; } = new();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SubGroups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Input = await _api.GetItemAsync(id) ?? new ItemDto();

        Categories = (await _api.GetCategoriesAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

        Groups = (await _api.GetGroupsAsync())
            .Select(g => new SelectListItem(g.Name, g.Id.ToString()));

        SubGroups = (await _api.GetSubGroupsAsync())
            .Select(sg => new SelectListItem(sg.Name, sg.Id.ToString()));
        ItemType = (await _api.GetItemTypeAsync())
            .Select(sg => new SelectListItem(sg.Name, sg.Id.ToString()));
        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await _api.UpdateItemAsync(Input);
        return RedirectToPage("Index");
    }
}
