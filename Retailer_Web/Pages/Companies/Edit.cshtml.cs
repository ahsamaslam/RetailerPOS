using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;
using System;

namespace Retailer.POS.Web.Pages.Companies
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) { _api = api; }

        [BindProperty]
        public CompanyDto Company { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dto = await _api.GetCompanyByIdAsync(id);
            if (dto == null) return NotFound();
            Company = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Company.Id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Company id is missing.");
                return Page();
            }

            var result = await _api.UpdateCompanyAsync(Company.Id, Company);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Unable to update company.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
