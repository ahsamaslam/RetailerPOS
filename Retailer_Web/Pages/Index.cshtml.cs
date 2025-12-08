using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;
[Authorize]
public class IndexModel : BasePageModel
{
    public IndexModel(IApiClient api) : base(api)
    {
    }

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