using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Sales
{
    [Authorize]
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new()
        {
            Details = new List<SalesDetailDto> { new SalesDetailDto() }
        };
        public CompanyDto company { get; set; } = new();
        public List<SelectListItem> SaleType { get; set; } = new List<SelectListItem>() { new SelectListItem {  Value="1", Text="Cash"}
        , new SelectListItem { Value = "1", Text = "Credit" } };
        public List<SelectListItem> CustomersList { get; set; } = new();
        public List<SelectListItem> CategoryList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            company = await _api.GetUserCompanyAsync();

            var customers = await _api.GetCustomersAsync();
            CustomersList = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            var catlst = await _api.GetCategoriesAsync();
            CategoryList = catlst.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            if (id.HasValue)
            {
                Sale = await _api.GetSaleByIdAsync(id.Value);
                if (Sale == null) return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetItemLookupAsync( int catId ,string term = "", int take = 20)
            => new JsonResult(await _api.SearchItemsAsync(catId,term, take));

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            Sale.CustomerID = Sale.CustomerCode??0;
            // Recalculate totals
            Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
            Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
            Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
            foreach (var item in Sale.Details)
            {
                item.ItemId = item.ItemCode;
            }
            Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

            bool success = false;
            SalesMasterDto data = new SalesMasterDto();
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
        }
    }
}
