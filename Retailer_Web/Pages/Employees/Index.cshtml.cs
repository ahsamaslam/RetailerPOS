using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;
namespace Retailer.POS.Web.Pages.Employees;
public class IndexModel : PageModel
{
    private readonly IApiClient _api;
    public IndexModel(IApiClient api) => _api = api;

    public List<EmployeeViewModel> Employees { get; set; } = new();

    public async Task OnGetAsync()
    {
        Employees = await _api.GetEmployeesAsync();
    }
}
