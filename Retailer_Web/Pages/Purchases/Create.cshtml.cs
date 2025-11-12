using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Purchases;
public class CreateModel : PageModel
{
    private readonly IApiClient _api;
    public CreatePurchaseDto Input { get; set; } = new();


    public CreateModel(IApiClient api) => _api = api;
    [BindProperty]
    public PurchaseMasterDto Purchase { get; set; } = new();
    public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> vendor { get; set; } = new List<SelectListItem>();
    public async Task OnGetAsync() { 
        Items = (await _api.GetItemsAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        vendor = (await _api.GetVendorsAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
    }
    public async Task<IActionResult> OnPostAsync()
    {   
        Purchase.SubTotal = Purchase.Details.Sum(d => d.Qty * d.Rate);
        Purchase.Total = Purchase.SubTotal; // Add tax/discount logic if needed
        if (!ModelState.IsValid) return Page();
        Input.Date = Purchase.Date;
        Input.VendorID = Purchase.VendorID; 
        Input.SubTotal = Purchase.SubTotal;
        Input.Total = Purchase.Total;
        Input.BranchId = 1;
       Input.LoginId = 2;
        Input.Details = Purchase.Details.Select(x=> new CreatePurchaseDetailDto { ItemId = x.ItemId, Discount =0, ItemName=x.ItemName ,Rate=x.Rate, Qty=x.Qty, TaxPercentage=0}).ToList();
        await _api.CreatePurchaseAsync(Input);
        return RedirectToPage("/Index");
    }
}
