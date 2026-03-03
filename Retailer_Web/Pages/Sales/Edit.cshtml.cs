using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales;
public class EditModel : PageModel
{
    private readonly IApiClient _api;
    public EditModel(IApiClient api) => _api = api;

    [BindProperty]
    public SalesMasterDto Sale { get; set; } = new();

    public List<SelectListItem> SaleType { get; private set; } = BuildSaleTypeOptions();
    public List<SelectListItem> CustomersList { get; private set; } = new();
    public List<SelectListItem> CategoryList { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Sale = await _api.GetSaleByIdAsync(id);
        if (Sale == null) return NotFound();

        Sale.Details ??= new List<SalesDetailDto>();
        await PopulateCustomersAsync();
        await PopulateCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetItemLookupAsync(int catID ,string term = "", int take = 20)
        => new JsonResult(await _api.SearchItemsAsync(catID , term, take));

    public async Task<IActionResult> OnPostAsync()
    {
        SaleType = BuildSaleTypeOptions();
        await PopulateCustomersAsync();
        await PopulateCategoriesAsync();

        if (!ModelState.IsValid) return Page();

        Sale.Details ??= new List<SalesDetailDto>();
        Sale.SubTotal = Sale.Details.Sum(d => d.Amount);
        Sale.TaxAmount = Sale.Details.Sum(d => d.TaxAmount);
        Sale.TotalDiscount = Sale.Details.Sum(d => d.Discount);
        Sale.BalanceAmount = Sale.SubTotal - Sale.TotalDiscount + Sale.TaxAmount;

        var success = await _api.UpdateSaleAsync(Sale);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Unable to update sale.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateCustomersAsync()
    {
        var customers = await _api.GetCustomersAsync();
        CustomersList = customers
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();
    }

    private async Task PopulateCategoriesAsync()
    {
        var categories = await _api.GetCategoriesAsync();
        CategoryList = categories
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();
    }

    private static List<SelectListItem> BuildSaleTypeOptions() =>
        new()
        {
            new SelectListItem { Value = "1", Text = "Cash" },
            new SelectListItem { Value = "1", Text = "Credit" }
        };
}
