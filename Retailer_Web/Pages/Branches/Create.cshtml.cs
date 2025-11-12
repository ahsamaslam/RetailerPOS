using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Branches
{
    public class CreateModel : PageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api) => _api = api;

        [BindProperty]
        public BranchDto Branch { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _api.CreateBranchAsync(Branch);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to create branch.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
