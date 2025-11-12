using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Categories;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IToastNotification _toastNotification;
    private readonly IApiClient _api;
    public IndexModel(IApiClient api, ILogger<IndexModel> logger, IToastNotification toastNotification) {
        _api = api; 
        _logger = logger;
        _toastNotification = toastNotification;
    }
    public List<ItemCategoryViewModel> Categories { get; set; } = new();
    public async Task OnGetAsync() { Categories = await _api.GetCategoriesAsync();
       // _toastNotification.AddErrorToastMessage("Items Loaded"); 
    }
}
