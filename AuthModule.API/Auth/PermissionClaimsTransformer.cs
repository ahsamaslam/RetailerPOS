using AuthModule.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthModule.API.Auth
{
    public class PermissionClaimsTransformer : IClaimsTransformation
    {
        private readonly ApplicationDbContext _db;
        public PermissionClaimsTransformer(ApplicationDbContext db) => _db = db;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity!.IsAuthenticated) return principal;

            var identity = (ClaimsIdentity)principal.Identity;
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return principal;

            // gather role names from claims
            var roleNames = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roleNames.Any())
            {
                // user may have roles stored elsewhere; skip if none
            }

            // map role names -> role ids (string)
            var roleEntities = await _db.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();
            var roleIds = roleEntities.Select(r => r.Id).ToList();

            // gather permission names from rolepermissions and userpermissions
            var permFromRoles = _db.RolePermissions
                                  .Where(rp => roleIds.Contains(rp.RoleId))
                                  .Select(rp => rp.Permission!.Name);

            var permFromUser = _db.UserPermissions
                                  .Where(up => up.UserId == userId)
                                  .Select(up => up.Permission!.Name);

            var permissionNames = await permFromRoles.Union(permFromUser).Distinct().ToListAsync();

            // add 'permission' claims if not already present
            var existing = identity.FindAll("permission").Select(c => c.Value).ToHashSet();
            foreach (var p in permissionNames)
            {
                if (!existing.Contains(p))
                    identity.AddClaim(new Claim("permission", p));
            }

            return principal;
        }
    }
}
