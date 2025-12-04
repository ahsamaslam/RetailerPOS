using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages; 
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Purchases;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;
    [BindProperty(SupportsGet = true)]
    public DateTime sdate { get; set; } = DateTime.Now;

    [BindProperty(SupportsGet = true)]
    public DateTime edate { get; set; } = DateTime.Now;
    public List<PurchaseViewModel> Purchase { get; set; } = new();
    public async Task OnGetAsync() => Purchase = await _api.GetPurchaseDateWiseAsync(sdate,edate);
}
