using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Retailer.POS.Api.Services;
public class TokenDelegationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenDelegationHandler> _logger;

    public TokenDelegationHandler(IHttpContextAccessor httpContextAccessor,
                                  ILogger<TokenDelegationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            _logger.LogDebug("No HttpContext available in TokenDelegationHandler (likely background thread). Skipping token forwarding.");
            return base.SendAsync(request, cancellationToken);
        }

        // 1) Prefer to forward incoming Authorization header if present (most common)
        var incomingAuth = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(incomingAuth))
        {
            // incomingAuth usually "Bearer <token>"
            if (AuthenticationHeaderValue.TryParse(incomingAuth, out var headerVal) &&
                headerVal.Scheme.Equals("Bearer", System.StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(headerVal.Parameter))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headerVal.Parameter);
                _logger.LogDebug("Forwarded Bearer token from incoming Authorization header.");
                return base.SendAsync(request, cancellationToken);
            }
        }

        // 2) Try session (if you store token there)
        try
        {
            var tokenFromSession = httpContext.Session?.GetString("JWT_TOKEN");
            if (!string.IsNullOrWhiteSpace(tokenFromSession))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromSession);
                _logger.LogDebug("Forwarded token from session (JWT_TOKEN).");
                return base.SendAsync(request, cancellationToken);
            }
        }
        catch (System.Exception ex)
        {
            // session may not be configured; don't fail the request, but log
            _logger.LogDebug(ex, "Error when attempting to read token from session.");
        }

        // 3) Try claim (if you stored token in a claim e.g., during login)
        var claimToken = httpContext.User?.FindFirst("access_token")?.Value
                      ?? httpContext.User?.FindFirst("token")?.Value;
        if (!string.IsNullOrWhiteSpace(claimToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", claimToken);
            _logger.LogDebug("Forwarded token from user claims (access_token/token).");
            return base.SendAsync(request, cancellationToken);
        }

        // 4) Try HttpContext.Items (if you put token there earlier)
        if (httpContext.Items.TryGetValue("JWT_TOKEN", out var itemVal) && itemVal is string tokenFromItems && !string.IsNullOrWhiteSpace(tokenFromItems))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromItems);
            _logger.LogDebug("Forwarded token from HttpContext.Items[\"JWT_TOKEN\"].");
            return base.SendAsync(request, cancellationToken);
        }

        // 5) Try cookie (if token stored in cookie) - name "jwt" or adjust as needed
        var cookieToken = httpContext.Request.Cookies["JWT_TOKEN"] ?? httpContext.Request.Cookies["jwt"];
        if (!string.IsNullOrWhiteSpace(cookieToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cookieToken);
            _logger.LogDebug("Forwarded token from cookie.");
            return base.SendAsync(request, cancellationToken);
        }

        // Nothing found — log and continue without Authorization header
        _logger.LogDebug("No token found to forward in TokenDelegationHandler.");
        return base.SendAsync(request, cancellationToken);
    }
}
