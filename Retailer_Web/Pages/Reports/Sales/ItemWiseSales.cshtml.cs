using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Retailer.Api.Helpers;
using Retailer.POS.Web.Services;
using Retailer.Web.ReportDto;

namespace Retailer.Web.Pages.Report.Sales;

[Authorize]
public class ItemWiseSalesModel : PageModel
{
    private readonly IApiClient _api;

    public ItemWiseSalesModel(IApiClient api)
    {
        _api = api;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? sdate { get; set; } = DateTime.Now;

    [BindProperty(SupportsGet = true)]
    public DateTime? edate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ItemCode { get; set; }

    public List<SelectListItem> Items { get; set; } = new();
    public List<string> Actions { get; } = new() { "Excel", "PDF", "Word" };

    public async Task<IActionResult> OnGet()
    {
        await LoadItems();
        return Page();
    }

    private async Task LoadItems()
    {
        var items = await _api.GetItemsAsync();
        Items = items.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
    }

    public async Task<IActionResult> OnGetLoadSales(int? itemCode, DateTime? sdate, DateTime? edate)
    {
        if (!itemCode.HasValue || !sdate.HasValue || !edate.HasValue)
        {
            return new JsonResult(new List<ItemSalesReport>());
        }

        var data = await _api.GetSalesReturnItemWiseAsync(itemCode.Value, sdate.Value, edate.Value);
        return new JsonResult(data);
    }

    public async Task<IActionResult> OnGetExportAsync(string export)
    {
        if (!sdate.HasValue || !edate.HasValue)
        {
            return BadRequest("Dates required.");
        }

        if (!ItemCode.HasValue)
        {
            return BadRequest("Item required.");
        }

        var bytes = await _api.ExportSalesReturnItemWiseAsync(ItemCode.Value, export, sdate.Value, edate.Value);
        var file = ExportFileResolver.Resolve(export, "SalesReturnReport");
        return File(bytes, file.ContentType, file.FileName);
    }
}
