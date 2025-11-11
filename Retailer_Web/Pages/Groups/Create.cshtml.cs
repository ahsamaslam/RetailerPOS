using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Groups;

public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreateModel(IApiClient api) => _api = api;

    [BindProperty]
    public ItemGroupViewModel Group { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var success = await _api.CreateGroupAsync(Group);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Unable to create group. Please try again.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
