using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Models;

namespace Retailer.Web.Pages.PurchaseReturn
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _api;
        public IndexModel(IApiClient api) => _api = api;
        [BindProperty(SupportsGet = true)]
        public DateTime sdate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public DateTime edate { get; set; } = DateTime.Now;
        public List<PurchaseReturnViewModel> Purchase { get; set; } = new();
        public async Task OnGetAsync() => Purchase = await _api.GetPurchaseReturnDateWiseAsync(sdate, edate);

        // ✅ AJAX Soft Delete Handler
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var payment = await _api.DeletePurchaseReturnAsync(id);

            if (!payment)
                return new JsonResult(new { success = false });

            return new JsonResult(new { success = true });
        }
    }
}
