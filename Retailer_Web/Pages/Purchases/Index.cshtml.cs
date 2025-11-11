using Microsoft.AspNetCore.Mvc.RazorPages; 
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Purchases;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;
    public List<PurchaseViewModel> Purchase { get; set; } = new();
    public async Task OnGetAsync() => Purchase = await _api.GetPurchasesAsync();
}
