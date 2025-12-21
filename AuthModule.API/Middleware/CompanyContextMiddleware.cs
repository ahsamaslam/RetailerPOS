using System.Security.Claims;

namespace AuthModule.API.Middleware;

public sealed class CompanyContextMiddleware
{
    private readonly RequestDelegate _next;

    public CompanyContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var principal = context.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        Guid? companyId = null;

        if (principal.IsInRole("superadmin"))
        {
            var headerValue = context.Request.Headers["X-Company-Id"].FirstOrDefault();
            if (Guid.TryParse(headerValue, out var parsed))
            {
                companyId = parsed;
            }
        }
        else
        {
            var claimValue = principal.FindFirst("companyId")?.Value;
            if (Guid.TryParse(claimValue, out var parsed))
            {
                companyId = parsed;
            }
        }

        if (companyId == null)
        {
            await _next(context);
            return;
        }

        context.Items["CompanyId"] = companyId.Value;
        await _next(context);
    }
}