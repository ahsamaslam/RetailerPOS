using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Dtos;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Sales
{
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) : base(api) => _api = api;

        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new()
        {
            Details = new List<SalesDetailDto> { new SalesDetailDto() }
        };

        public List<ItemSelectListItem> ItemsList { get; set; } = new();
        public List<SelectListItem> SaleType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
        , new SelectListItem { Value = "1", Text = "Credit" } };
        public List<SelectListItem> CustomersList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Load dropdown data
            var items = await _api.GetItemsAsync();
            ItemsList = items.Select(i => new ItemSelectListItem { Value = i.Id.ToString(), Text = i.Name , rate = i.Rate, cost=i.Cost, qty =  i.QtyInHand }).ToList();

            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            if (id.HasValue)
            {
                Sale = await _api.GetSaleByIdAsync(id.Value);
                if (Sale == null) return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Recalculate totals
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            bool success=false;
            SalesMasterDto data = new SalesMasterDto() ;
            if (Sale.Id > 0)
            {
                success = await _api.UpdateSaleAsync(Sale);


                if (!success)
                {
                    ModelState.AddModelError("", "Unable to save sale.");
                    return Page();
                }


            }
            else
            {
                data = await _api.CreateSaleAsync(Sale);
            }
          
            return Redirect($"~/Sales/Print/{data.Id}");
         //   return RedirectToPage("Index");
        }
    }
}
