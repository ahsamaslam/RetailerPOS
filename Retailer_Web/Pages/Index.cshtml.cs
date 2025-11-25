using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
[Authorize]
public class IndexModel : PageModel
{
    private readonly IApiClient _api;

    public IndexModel(IApiClient api) => _api = api;

    public IEnumerable<MenuDto> Menus { get; set; } = Enumerable.Empty<MenuDto>();
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
            Menus = await _api.GetMenusForCurrentUserAsync();
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
            // log if you have logger; for now show empty menu and page
            // _logger?.LogError(ex, "Failed to load menus");
            Menus = Enumerable.Empty<MenuDto>();
            return Page();
        }
    }
}