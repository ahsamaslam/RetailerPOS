using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;
using Retailer.Web.Pages;

namespace Retailer.POS.Web.Pages.Branches
{
    [Authorize]
    public class CreateModel : BasePageModel
    {
        private readonly IApiClient _api;
        public CreateModel(IApiClient api)
        {
            _api = api;
        }

        [BindProperty]
        public BranchDto Branch { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _api.CreateBranchAsync(Branch);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(result.Message) ? "Unable to create branch." : result.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
