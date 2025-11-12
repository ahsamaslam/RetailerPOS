using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Models;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;

namespace Retailer.POS.Web.Pages.Branches
{
    public class EditModel : PageModel
    {
        private readonly IApiClient _api;
        public EditModel(IApiClient api) => _api = api;

        [BindProperty]
        public BranchDto Branch { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _api.GetBranchByIdAsync(id);
            if (dto == null) return NotFound();
            Branch = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _api.UpdateBranchAsync(Branch);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update branch.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
