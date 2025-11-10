using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Items;
public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreateItemDto Input { get; set; } = new();
    public CreateModel(IApiClient api) => _api = api;
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.CreateItemAsync(Input);
        return RedirectToPage("Index");
    }
}
