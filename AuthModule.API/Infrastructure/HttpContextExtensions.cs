using AuthModule.API.Dtos;
using System.Security.Claims;

namespace AuthModule.Infrastructure
{
    public static class HttpContextExtensions
    {
        public static Guid GetCompanyId(this HttpContext context)
        {
            if (context.Items.TryGetValue("CompanyId", out var value) &&
                value is Guid companyId)
            {
                return companyId;
            }

            throw new UnauthorizedAccessException("Company context not resolved.");
        }
        public static UserDto GetUserId(this HttpContext context)
        {
            if (context?.User?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var user = context.User;

            // ---- User Id ----
            var userId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userId, out var guid))
                throw new UnauthorizedAccessException("User Id not found in token.");

            // ---- Username ----
            var userName =
                user.FindFirst(ClaimTypes.Name)?.Value
                ?? user.FindFirst("username")?.Value
                ?? string.Empty;

            // ---- Roles ----
            var roles = user.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value)
                            .Distinct()
                            .ToList();

            return new UserDto
            {
                Id = guid.ToString(),
                UserName = userName,
                Roles = roles
            };
        }
    }
}
