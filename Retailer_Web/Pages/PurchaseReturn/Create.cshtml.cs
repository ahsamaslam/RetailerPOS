using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.Web.Pages.PurchaseReturn
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreatePurchaseReturnDto Input { get; set; } = new();


        public CreateModel(IApiClient api) => _api = api;
        [BindProperty]
        public PurchaseReturnMasterDto Purchase { get; set; } = new()
        {
            Details = new List<PurchaseReturnDetailDto> { new PurchaseReturnDetailDto() }
        };
        public IEnumerable<SelectListItem> PurchaseType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
        , new SelectListItem { Value = "1", Text = "Credit" } };
        public IEnumerable<SelectListItem> ItemsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> vendorList { get; set; } = new List<SelectListItem>();
        public async Task OnGetAsync()
        {
            ItemsList = (await _api.GetItemsAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            vendorList = (await _api.GetVendorsAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        }

        public async Task<IActionResult> OnGetItemLookupAsync(string term = "", int take = 20)
            => new JsonResult(await _api.SearchItemsAsync(term, take));

        public async Task<IActionResult> OnPostAsync()
        {
            Purchase.SubTotal = Purchase.Details.Sum(d => d.Qty * d.Rate);
            Purchase.Total = Purchase.SubTotal; // Add tax/discount logic if needed
            if (!ModelState.IsValid) return Page();
            Input.Date = Purchase.Date;
            Input.VendorID = Purchase.VendorID;
            Input.SubTotal = Purchase.SubTotal;
            Input.Total = Purchase.Total;
            Input.PurchaseType = Purchase.PurchaseType;
            Input.BranchId = 1;
            Input.LoginId = 2;
            Input.Details = Purchase.Details.Select(x => new CreatePurchaseReturnDetailDto { ItemId = x.ItemId, Discount = 0, ItemName = x.ItemName, Rate = x.Rate, Qty = x.Qty, TaxPercentage = 0 }).ToList();
            await _api.CreatePurchaseReturnAsync(Input);
            return RedirectToPage("/PurchaseReturn/Index");
        }
    }

}
