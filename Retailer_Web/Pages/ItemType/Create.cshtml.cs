using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.ItemType;
[Authorize]
public class CreateModel : BasePageModel
{
    private readonly IApiClient _api;
    public CreateModel(IApiClient api) { _api = api; }

    [BindProperty]
    public ItemTypeViewModel ItemType { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    { 
        if (!ModelState.IsValid) return Page();
        (bool Success, string Message) =await _api.CreateItemTypeAsync(ItemType);

        if (!Success)
        {
            ModelState.AddModelError(string.Empty, Message);
            return Page();
        }


        return RedirectToPage("Index");
    }
}
