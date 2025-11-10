using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Employees
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api) => _api = api;

        public EmployeeViewModel Employee { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var emp = await _api.GetEmployeeByIdAsync(id);
            if (emp != null) Employee = emp;
        }
    }
}
