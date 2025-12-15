using AuthModule.API.Auth;
using Microsoft.AspNetCore.Authorization;
namespace Retailer.Api.Data
{

    public class ClaimsPermissionHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;
            var endpoint = httpContext?.GetEndpoint();
            var required = endpoint?.Metadata.GetMetadata<RequiresPermissionAttribute>();

            if (required == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var permission = required.Permission;

            if (context.User.HasClaim("permission", permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

}
