using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.Services;
using Retailer.POS.Web.ApiDTOs;
using System.Text.RegularExpressions;
using Retailer.Web.Pages;
using Microsoft.AspNetCore.Authorization;
using Retailer.POS.Web.Models;

namespace Retailer.POS.Web.Pages.Items;
[Authorize]
public class CreateModel : BasePageModel
{
    private readonly IApiClient _api;

    public CreateModel(IApiClient api) { _api = api; }

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

    // AJAX handler for creating Item Type
    public async Task<IActionResult> OnPostCreateItemTypeAsync([FromBody] ItemTypeCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
        {
            return new JsonResult(new { success = false, message = "Type name is required" });
        }

        var itemType = new ItemTypeViewModel { Name = request.Name.Trim() };
        var (Success, Message) = await _api.CreateItemTypeAsync(itemType);

        if (Success)
        {
            // Get the newly created item type ID by fetching all and finding the one with matching name
            var allTypes = await _api.GetItemTypeAsync();
            var newType = allTypes.FirstOrDefault(t => t.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            return new JsonResult(new 
            { 
                success = true, 
                id = newType?.Id ?? 0, 
                name = newType?.Name ?? request.Name 
            });
        }

        return new JsonResult(new { success = false, message = Message });
    }

    // Helper class for AJAX request
    public class ItemTypeCreateRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
