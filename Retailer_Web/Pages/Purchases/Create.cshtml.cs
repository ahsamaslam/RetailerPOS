using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.DTOs;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Purchases;
public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreatePurchaseDto Input { get; set; } = new();
    public CreateModel(IApiClient api) => _api = api;
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _api.CreatePurchaseAsync(Input);
        return RedirectToPage("/Index");
    }
}
