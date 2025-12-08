using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
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

        var client = _httpFactory.CreateClient("AuthApi");
        var resp = await client.PostAsJsonAsync("api/auth/login", new { UserName, Password }); 
        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return Page();
        }

        // Read JSON response and extract token (AuthController returns { token: "...", expiresIn:..., roles: ... })
        var root = await resp.Content.ReadFromJsonAsync<JsonElement?>();
        if (root == null || !root.Value.TryGetProperty("token", out var tokenElem))
        {
            ModelState.AddModelError("", "Invalid auth server response (no token)");
            return Page();
        }

        var token = tokenElem.GetString();
        if (string.IsNullOrEmpty(token))
        {
            ModelState.AddModelError("", "Empty token returned by auth server");
            return Page();
        }

        // Decode JWT to extract claims (sub, name, roles, permission, etc.)
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;
        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Failed to parse token from auth server");
            return Page();
        }

        // get user id (sub or nameidentifier)
        var userId = jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        var usernameClaim = jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.Name || c.Type == "name" || c.Type == JwtRegisteredClaimNames.Name)?.Value
            ?? UserName;

        // collect role claims (tokens sometimes have role claim type as ClaimTypes.Role or "role" or that long URI)
        var roleClaimTypes = new[] { ClaimTypes.Role, "role", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" };
        var roles = jwt.Claims
            .Where(c => roleClaimTypes.Contains(c.Type, StringComparer.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // collect permission claims (your token contains "permission" array)
        var permissions = jwt.Claims
            .Where(c => string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // Build claims for cookie principal — include token as access_token claim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId ?? string.Empty),
            new Claim(ClaimTypes.Name, usernameClaim),
            new Claim("access_token", token),  // TokenDelegatingHandler will read this claim if needed
            new Claim("sub", userId ?? string.Empty)
        };

        // add roles
        foreach (var r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        // add permission claims
        foreach (var p in permissions)
            claims.Add(new Claim("permission", p));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // store access token in session (TokenDelegatingHandler reads session first)
        HttpContext.Session.SetString("AccessToken", token);
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<LoginModel>>();
        logger.LogInformation("Login completed for {user}. Token length={len}", UserName, token?.Length);
        // sign-in cookie so user is authenticated in Razor pages
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return LocalRedirect(returnUrl);
    }
}
