using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Employees;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IEnumerable<dynamic> Items { get; set; } = Enumerable.Empty<dynamic>();
    public IndexModel(IApiClient api) => _api = api;
    public async Task OnGetAsync() => Items = await _api.GetEmployeesAsync();
}
