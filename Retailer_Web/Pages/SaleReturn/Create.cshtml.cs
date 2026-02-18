using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Dtos;

namespace Retailer.Web.Pages.SaleReturn
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesMasterReturnDto Sale { get; set; } = new()
        {
            Details = new List<SalesReturnDetailDto> { new SalesReturnDetailDto() }
        };
        public CompanyDto company { get; set; } = new();
        public List<ItemSelectListItem> ItemsList { get; set; } = new();
        public List<SelectListItem> SaleType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
        , new SelectListItem { Value = "1", Text = "Credit" } };
        public List<SelectListItem> CustomersList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            company = await _api.GetUserCompanyAsync();
            // Load dropdown data
            var items = await _api.GetItemsAsync();
            ItemsList = items.Select(i => new ItemSelectListItem { Value = i.Id.ToString(), Text = i.Name, rate = i.Rate, cost = i.Cost, qty = i.QtyInHand }).ToList();

            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            if (id.HasValue)
            {
                Sale = await _api.GetSaleReturnByIdAsync(id.Value);
                if (Sale == null) return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetItemLookupAsync(string term = "", int take = 20)
            => new JsonResult(await _api.SearchItemsAsync(term, take));

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Recalculate totals
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            bool success = false;
            SalesMasterReturnDto data = new SalesMasterReturnDto();
          
            
            {
                data = await _api.CreateSaleAsync(Sale);
            }

            return Redirect($"~/SaleReturn/");
            //   return RedirectToPage("Index");
        }
    }
}
