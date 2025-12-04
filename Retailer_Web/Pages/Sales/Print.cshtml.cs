using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.ApiDTOs;
using Retailer.POS.Web.Services;

namespace Retailer.POS.Web.Pages.Sales;

    public class PrintModel : PageModel
    {
        private readonly IApiClient _api;
        public PrintModel(IApiClient api) => _api = api;
        [BindProperty]
        public SalesMasterDto Sale { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Sale = await _api.GetSaleByIdAsync(id); // fetch DTO from API
            if (Sale == null) return NotFound();
            return Page();
        }
    
}
