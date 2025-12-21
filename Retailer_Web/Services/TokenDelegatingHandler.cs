using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Retailer.POS.Web.Services;
public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenDelegatingHandler> _logger;

    public TokenDelegatingHandler(IHttpContextAccessor httpContextAccessor,
                                  ILogger<TokenDelegatingHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var ctx = _httpContextAccessor.HttpContext;

            if (ctx == null)
            {
                _logger.LogDebug("No HttpContext available for TokenDelegatingHandler.");
                return base.SendAsync(request, cancellationToken);
            }

            // 1) Try session (preferred)
            string? token = null;
            try
            {
                token = ctx.Session?.GetString("AccessToken");
            }
            catch (Exception ex)
            {
                // session might not be enabled; log and continue
                _logger.LogDebug(ex, "Session read failed in TokenDelegatingHandler.");
            }

            // 2) Fallback: token stored as claim on the principal
            if (string.IsNullOrEmpty(token))
            {
                token = ctx.User?.FindFirst("access_token")?.Value;
            }

            // 3) Fallback: token stored in cookie (if you used a cookie to persist it)
            if (string.IsNullOrEmpty(token) && ctx.Request.Cookies.TryGetValue("access_token", out var cookieToken))
            {
                token = cookieToken;
            }

            if (!string.IsNullOrEmpty(token))
            {
                // only set header if not already set
                if (request.Headers.Authorization == null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug("Attached Bearer token to outgoing request: {Method} {Uri}", request.Method, request.RequestUri);
                }
                var companyId = ctx.Session?.GetString("ImpersonatedCompanyId");
                if (!string.IsNullOrEmpty(companyId))
                    request.Headers.Add("X-Company-Id", companyId);
            }
            else
            {
                _logger.LogDebug("No token found to attach for outgoing request {Method} {Uri}", request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            // swallow and continue — don't break outgoing requests if handler fails
            _logger.LogError(ex, "TokenDelegatingHandler failed while trying to attach token");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
