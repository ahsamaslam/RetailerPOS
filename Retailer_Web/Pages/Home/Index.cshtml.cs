using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages.Home
{

    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public double totalSale { get; set; } = 20000;
        [BindProperty]
        public double totalSaleReturn { get; set; } = 20000;
        [BindProperty]
        public int[] SaleSeries { get; set; } = new[] { 700, 100 };
        [BindProperty]

        public string[] SaleLabels { get; set; } = new[] { "Sale", "Return" };
        public int[] PurchaseSeries { get; set; } = new[] { 1500, 100 };
        [BindProperty]
        public string[] PurchaseLabels { get; set; } = new[] { "Purchase", "Return" };
        public int[] PaymentSeries { get; set; } = new[] { 15000, 1500 };
        [BindProperty]
        public string[] PaymentLabels { get; set; } = new[] { "Customer", "Vendor" };

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Optional: clear any session data if you used Session
            //     HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // if not authenticated redirect to login (safe-guard)
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Login", new { ReturnUrl = Url.Content("~/") });
            }

            try
            {
                return Page();
            }
            catch (UnauthorizedAccessException)
            {
                // token expired or missing — sign out cookie and redirect to login
                await HttpContext.SignOutAsync(); // requires using Microsoft.AspNetCore.Authentication
                return RedirectToPage("/Login", new { ReturnUrl = Url.Content("~/") });
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}