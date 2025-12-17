using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Employees
{
    [Authorize]
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) { _api = api; }

        [BindProperty]
        public EmployeeDto Employee { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var success = await _api.CreateEmployeeAsync(Employee);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create employee");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
