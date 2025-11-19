using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages;
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpFactory;
    public LoginModel(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [BindProperty]
    public string UserName { get; set; } = string.Empty;
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl ?? Url.Content("~/");

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        var client = _httpFactory.CreateClient("AuthApi"); // optional, or call absolute URL
        // call auth module /api/auth/login (adjust path as your AuthModule exposes)
        var resp = await client.PostAsJsonAsync("api/auth/login", new { UserName, Password });

        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return Page();
        }

        var loginResult = await resp.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (loginResult == null)
        {
            ModelState.AddModelError("", "Invalid auth server response");
            return Page();
        }

        // Build claims (id, name, roles, permissions, store token as access_token)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, loginResult.UserId),
            new Claim(ClaimTypes.Name, loginResult.UserName ?? UserName),
            new Claim("access_token", loginResult.Token),
            new Claim("sub", loginResult.UserId)
        };

        if (loginResult.Roles != null)
            claims.AddRange(loginResult.Roles.Distinct().Select(r => new Claim(ClaimTypes.Role, r)));

        if (loginResult.Permissions != null)
            claims.AddRange(loginResult.Permissions.Distinct().Select(p => new Claim("permission", p)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

        return LocalRedirect(returnUrl);
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;     // JWT from AuthModule
        public string UserId { get; set; } = string.Empty;    // identity user id
        public string? UserName { get; set; }
        public string[]? Roles { get; set; }
        public string[]? Permissions { get; set; }
    }
}
