using System.Security.Claims;

namespace Retailer.Api.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            // Not authenticated → skip (Auth middleware will handle)
            if (user?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userName =
                user.FindFirst(ClaimTypes.Name)?.Value ??
                user.FindFirst("username")?.Value ??
                user.Identity?.Name ??
                string.Empty;

            var isSystemUser = string.Equals(userName, "System", StringComparison.OrdinalIgnoreCase);
            var isSuperAdmin = user.IsInRole("superadmin") || isSystemUser;

            Guid? companyId = null;

            if (isSuperAdmin)
            {
                // SuperAdmin: company comes from header
                var headerValue = context.Request.Headers["X-Company-Id"].FirstOrDefault();
                if (Guid.TryParse(headerValue, out var parsed))
                {
                    companyId = parsed;
                }
                else if (isSystemUser)
                {
                    // allow system user to proceed even without company context
                    companyId = Guid.Empty;
                }
            }
            else
            {
                // Normal user: company comes from JWT
                var claimValue = user.FindFirst("companyId")?.Value;
                if (Guid.TryParse(claimValue, out var parsed))
                {
                    companyId = parsed;
                }
            }

            // Enforce company context for business APIs
            if (companyId == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Company context is missing.");
                return;
            }

            // Store resolved company in HttpContext for later use
            context.Items["CompanyId"] = companyId;

            await _next(context);
        }
    }
}
