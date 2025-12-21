using System.IdentityModel.Tokens.Jwt;
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
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        IHttpClientFactory httpFactory,
        ILogger<LoginModel> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    [BindProperty]
    public string UserName { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = "~/";

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "~/";
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "~/";

        var client = _httpFactory.CreateClient("AuthApi");
        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new { UserName, Password });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return Page();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (!json.TryGetProperty("token", out var tokenElement))
        {
            ModelState.AddModelError(string.Empty, "Authentication server returned no token");
            return Page();
        }

        var token = tokenElement.GetString();
        if (string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError(string.Empty, "Empty token returned from authentication server");
            return Page();
        }

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;

        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Invalid token format");
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,
                jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value),

            new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Name || c.Type == "name")?.Value
                ?? UserName),

            new Claim("access_token", token)
        };

        var roleValues = jwt.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var role in roleValues)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var perm in jwt.Claims
                     .Where(c => c.Type == "permission")
                     .Select(c => c.Value)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim("permission", perm));
        }

        var isSuperAdmin = roleValues.Any(r => string.Equals(r, "superadmin", StringComparison.OrdinalIgnoreCase));
        ReturnUrl = isSuperAdmin
            ? "/SuperAdmin/SwitchCompany"
            : returnUrl ?? "~/Home";

        var picture = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
        if (!string.IsNullOrWhiteSpace(picture))
        {
            claims.Add(new Claim("picture", picture));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        HttpContext.Session.Remove("ImpersonatedCompanyId");
        HttpContext.Session.Remove("ImpersonatedCompanyName");
        HttpContext.Session.SetString("AccessToken", token);

        _logger.LogInformation("User {User} logged in successfully", UserName);

        return LocalRedirect(ReturnUrl);
    }
}