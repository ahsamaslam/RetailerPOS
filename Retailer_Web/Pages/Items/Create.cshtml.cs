using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.Services;
using Retailer.POS.Web.ApiDTOs;
using System.Text.RegularExpressions;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Items;

public class CreateModel : BasePageModel
{
    private readonly IApiClient _api;

    public CreateModel(IApiClient api) : base(api) { _api = api; }

    [BindProperty]
    public CreateItemDto Input { get; set; } = new();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SubGroups { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ItemType { get; set; } = new List<SelectListItem>();

    public async Task OnGetAsync()
    {
        Categories = (await _api.GetCategoriesAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));

        Groups = (await _api.GetGroupsAsync())
            .Select(g => new SelectListItem(g.Name, g.Id.ToString()));

        SubGroups = (await _api.GetSubGroupsAsync())
            .Select(sg => new SelectListItem(sg.Name, sg.Id.ToString()));
        ItemType = (await _api.GetItemTypeAsync())
            .Select(sg => new SelectListItem(sg.Name, sg.Id.ToString()));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();  
        (bool Success, string Message) = await _api.CreateItemAsync(Input);

        if (!Success)
        {
            ModelState.AddModelError(string.Empty, Message);
            return Page();
        }


        return RedirectToPage("Index");
    }
}
