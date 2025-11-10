using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.POS.Web.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public EmployeeViewModel Employee { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var emp = await _api.GetEmployeeByIdAsync(id);
            if (emp == null) return NotFound();
            Employee = emp;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _api.UpdateEmployeeAsync(Employee);
            return RedirectToPage("Index");
        }
    }
}
