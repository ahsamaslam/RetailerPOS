using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuthModule.API.Services;

namespace AuthModule.API.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
                return;

            // Get the string userId from claims
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? context.User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return;

            // Call the correct method (string)
            var hasPermission =
                await _permissionService.UserHasPermissionAsync(userId, requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);
        }

    }
}
