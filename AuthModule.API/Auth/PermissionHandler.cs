using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuthModule.API.Services;
using AuthModule.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor,
                                 ApplicationDbContext db,
                                 UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Get required permission from endpoint metadata
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var endpoint = httpContext.GetEndpoint();
            var required = endpoint?.Metadata.GetMetadata<RequiresPermissionAttribute>();
            if (required == null || string.IsNullOrWhiteSpace(required.Permission))
            {
                // No permission metadata — deny by default (or you can choose to succeed)
                context.Fail();
                return;
            }

            var permission = required.Permission;

            // If user has permission claim, allow
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                // 1) check claims first
                if (user.HasClaim(c => c.Type == "permission" && c.Value == permission))
                {
                    context.Succeed(requirement);
                    return;
                }

                // 2) fallback: check DB (role -> rolePermission or userPermission)
                // get user id claim
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail();
                    return;
                }

                // get user's roles (from claims if present; else ask UserManager)
                var roleNames = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (!roleNames.Any())
                {
                    // fallback to userManager to get roles (costlier)
                    var identityUser = await _userManager.FindByIdAsync(userId);
                    if (identityUser != null)
                        roleNames = (await _userManager.GetRolesAsync(identityUser)).ToList();
                }

                // Map role names -> role ids (IdentityRole.Id is string)
                var roleIds = await _db.Roles
                    .Where(r => roleNames.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToListAsync();

                // Check RolePermissions: does any role have the permission name?
                var hasRolePermission = await _db.RolePermissions
                    .Include(rp => rp.Permission)
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .AnyAsync(rp => rp.Permission != null && rp.Permission.Name == permission);

                if (hasRolePermission)
                {
                    context.Succeed(requirement);
                    return;
                }

                // Check user specific permission
                var hasUserPermission = await _db.UserPermissions
                    .Include(up => up.Permission)
                    .AnyAsync(up => up.UserId == userId && up.Permission != null && up.Permission.Name == permission);

                if (hasUserPermission)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}
