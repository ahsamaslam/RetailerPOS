using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;
namespace Retailer.POS.Web.Pages.Purchases;
public class CreateModel : BasePageModel
{
    private readonly IApiClient _api;
    public CreatePurchaseDto Input { get; set; } = new();


    public CreateModel(IApiClient api) :base(api) => _api = api;
    [BindProperty]
    public PurchaseMasterDto Purchase { get; set; } = new()
    {
        Details = new List<PurchaseDetailDto> { new PurchaseDetailDto() }
    };
    public IEnumerable<SelectListItem> PurchaseType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
        , new SelectListItem { Value = "1", Text = "Credit" } };
    public IEnumerable<SelectListItem> ItemsList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> vendorList { get; set; } = new List<SelectListItem>();
    public async Task OnGetAsync() {
        ItemsList = (await _api.GetItemsAsync())
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        vendorList = (await _api.GetVendorsAsync())
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
        return RedirectToPage("/Purchases/Index");
    }
}
