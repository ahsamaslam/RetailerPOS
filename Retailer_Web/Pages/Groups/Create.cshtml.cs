using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Groups;

public class CreateModel : BasePageModel
{
    private readonly IApiClient _api;
    public CreateModel(IApiClient api) : base(api) { _api = api; }

    [BindProperty]
    public ItemGroupViewModel Group { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    { 
        if (!ModelState.IsValid) return Page();
        (bool Success, string Message) = await _api.CreateGroupAsync(Group);

        if (!Success)
        {
            ModelState.AddModelError(string.Empty, Message);
            return Page();
        }
       

        return RedirectToPage("Index");
    }
}
