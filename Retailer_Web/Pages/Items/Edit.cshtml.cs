using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Items;
[Authorize]
public class EditModel : BasePageModel
{
    private readonly IApiClient _api;

    public EditModel(IApiClient api) { _api = api; }

    [BindProperty]
    public ItemDto Input { get; set; } = new();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SubGroups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Input = await _api.GetItemAsync(id) ?? new ItemDto();
        await LoadListsAsync();
        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return Page();
        }

        await _api.UpdateItemAsync(Input);
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync()
    {
        Categories = (await _api.GetCategoriesAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == Input.CategoryId));

        Groups = (await _api.GetGroupsAsync())
            .Select(g => new SelectListItem(g.Name, g.Id.ToString(), g.Id == Input.GroupId));

        SubGroups = (await _api.GetSubGroupsAsync())
            .Select(sg => new SelectListItem(sg.Name, sg.Id.ToString(), Input.SubGroupId.HasValue && sg.Id == Input.SubGroupId.Value));

        ItemType = (await _api.GetItemTypeAsync())
            .Select(it => new SelectListItem(it.Name, it.Id.ToString(), it.Id == Input.ItemTypeId));
    }
}
