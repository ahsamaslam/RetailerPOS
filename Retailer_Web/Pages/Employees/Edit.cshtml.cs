using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.POS.Web.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public EmployeeDto Employee { get; set; } = new();

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

            var success = await _api.UpdateEmployeeAsync(Employee);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to update employee");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
