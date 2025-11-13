using Microsoft.AspNetCore.Authorization;

namespace AuthModule.API.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }


            var has = context.User.Claims.Any(c => c.Type == "permission" && c.Value == requirement.Permission);
            if (has) context.Succeed(requirement);
            else context.Fail();


            return Task.CompletedTask;
        }
    }
}
