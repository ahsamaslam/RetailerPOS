using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
using Retailer.Web.Pages;
namespace Retailer.POS.Web.Pages.Customers;
public class IndexModel : BasePageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) : base(api) { _api = api; }

    public List<CustomerViewModel> Customers { get; set; } = new();

    public async Task OnGetAsync()
    {
        Customers = await _api.GetCustomersAsync();
    }
}