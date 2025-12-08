using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.OpeningBalances;
public class IndexModel : BasePageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IToastNotification _toastNotification;
    private readonly IApiClient _api;

    public IndexModel(IApiClient api, ILogger<IndexModel> logger, IToastNotification toastNotification): base(api)
    {
        _api = api;
        _logger = logger;
        _toastNotification = toastNotification;
    }

    public List<OpeningBalanceViewModel> OpeningBalances { get; set; } = new();
    public List<ItemDto> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        OpeningBalances = await _api.GetOpeningBalancesAsync();

        // if OpeningBalanceViewModel doesn't include ProductName, fetch products and map names
        Products = await _api.GetItemsAsync();
        if (OpeningBalances.Any() && string.IsNullOrWhiteSpace(OpeningBalances.First().ProductName))
        {
            var prodMap = Products.ToDictionary(p => p.Id, p => p.Name);
            foreach (var ob in OpeningBalances)
            {
                if (prodMap.TryGetValue(ob.ProductId, out var name))
                    ob.ProductName = name;
            }
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _api.DeleteOpeningBalanceAsync(id);
        if (result.Success)
        {
            _toastNotification.AddSuccessToastMessage("Deleted successfully.");
        }
        else
        {
            var msg = result.ErrorMessage ?? "Error deleting record.";
            _toastNotification.AddErrorToastMessage(msg);
        }
        return RedirectToPage();
    }
}
