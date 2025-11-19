using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Retailer.POS.Web.Services;
public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var ctx = _httpContextAccessor.HttpContext;
        var token = ctx?.User?.FindFirst("access_token")?.Value; // we store token as claim on sign-in
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
