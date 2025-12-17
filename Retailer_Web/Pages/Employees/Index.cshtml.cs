using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Employees
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) { _api = api; }

        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();

        public async Task OnGetAsync()
        {
            Employees = await _api.GetEmployeesAsync();
        }
    }
}
