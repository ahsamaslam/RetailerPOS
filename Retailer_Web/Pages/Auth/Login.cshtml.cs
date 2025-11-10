using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
namespace Retailer.POS.Web.Pages.Auth;
public class LoginModel : PageModel
{
    private readonly IApiClient _api;
    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? Token { get; set; }
    public LoginModel(IApiClient api) => _api = api;
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        Token = await _api.LoginAsync(Username, Password);
        if (Token == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials");
            return Page();
        }
        Response.Cookies.Append("AuthToken", Token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddMinutes(60) });
        return Page();
    }
}
