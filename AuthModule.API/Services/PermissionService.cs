using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthModule.API.Data;
using AuthModule.API.Dtos;
using AuthModule.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthModule.API.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PermissionService>? _logger;
        private readonly UserManager<ApplicationUser> _userManager;



        public PermissionService(ApplicationDbContext db, IMemoryCache cache,
            ILogger<PermissionService>? logger, UserManager<ApplicationUser> userManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Permission> CreatePermissionAsync(string name, string? description = null)
        {
            var perm = new Permission { Name = name, Description = description };
            _db.Permissions.Add(perm);
            await _db.SaveChangesAsync();
            return perm;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _db.Permissions.AsNoTracking().ToListAsync();
        }

        public async Task AssignPermissionToRoleAsync(string roleId, int permissionId)
        {
            if (await _db.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)) return;
            _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            await _db.SaveChangesAsync();
        }
        public async Task AssignPermissionToUserAsync(string UserId, int permissionId)
        {
            if (await _db.UserPermissions.AnyAsync(rp => rp.UserId == UserId && rp.PermissionId == permissionId)) return;
            _db.UserPermissions.Add(new UserPermission { UserId = UserId, PermissionId = permissionId });
            await _db.SaveChangesAsync();
        }
        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, int permissionId)
        {
            var existing = await _db.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (existing == null) return false;
            _db.RolePermissions.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PermissionDto>> GetPermissionsForRoleAsync(string roleId)
        {
            return await _db.RolePermissions
                   .Where(rp => rp.RoleId == roleId && rp.Permission != null)
                   .Select(rp => new PermissionDto(
                       rp.Permission!.Id,
                       rp.Permission.Name,
                       rp.Permission.Description ?? string.Empty
                   ))
                   .ToListAsync();
        }
        public async Task<Permission?> GetPermissionAsync(int permissionId)
        {
            return await _db.Permissions
                  .FirstOrDefaultAsync(rp => rp.Id == permissionId);
        }
        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            var permission = await _db.Permissions.FindAsync(permissionId);
            if (permission == null) return false;
            _db.Permissions.Remove(permission);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<Permission>> GetPermissionsForDefaultUserAsync() {


            List<string> roles = new List<string> { "Purchase", "Sales",  };
          return  await _db.Permissions.Where(i => roles.Any(r => i.Name.Contains(r))).ToListAsync();


        }
        public async Task<List<string>> GetPermissionsForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return new List<string>();

            var userRolesSet = _db.Set<IdentityUserRole<string>>();

            var roleIds = await (from ur in userRolesSet
                                 where ur.UserId == userId
                                 select ur.RoleId).ToListAsync();

            // permissions from roles
            var rolePerms = new List<string>();
            if (roleIds.Count > 0)
            {
                rolePerms = await _db.RolePermissions
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .Select(rp => rp.Permission!.Name)
                    .ToListAsync();
            }

            // direct user permissions (with IsAllowed flag)
            var userPerms = await _db.UserPermissions
                .Where(up => up.UserId == userId)
                .Include(x => x.Permission)
                .Select(up => new { up.Permission!.Name, up.IsAllowed, link = up.Permission.link })
                .ToListAsync();

            var result = new HashSet<string>(rolePerms, StringComparer.OrdinalIgnoreCase);

            foreach (var up in userPerms)
            {
                if (up.IsAllowed) result.Add(up.Name);
                else result.RemoveWhere(n => string.Equals(n, up.Name, StringComparison.OrdinalIgnoreCase));
            }

            return result.ToList();
        }

        /// <summary>
        /// Check if user has the permission (via role or direct assignment).
        /// userId is string (Identity user id).
        /// </summary>
        public async Task<bool> UserHasPermissionAsync(string userId, string permission)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(permission))
                return false;

            permission = permission.Trim();

            // First, check cached computed permissions for user
            var perms = await GetPermissionsForUserAsync(userId);
            var found = perms.Any(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
            return found;
        }
        /// <summary>
        /// Remove a specific permission assignment for a user.
        /// Returns true when a row was removed, false when not found / nothing to remove.
        /// </summary>
        public async Task<bool> RemovePermissionFromUserAsync(string userId, int permissionId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            // Verify user exists (avoid orphaned userpermission rows)
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger?.LogWarning("RemovePermissionFromUserAsync: user {UserId} not found", userId);
                return false;
            }

            // Find the user-permission row
            var userPerm = await _db.UserPermissions
                                    .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            if (userPerm == null)
            {
                // nothing to remove
                _logger?.LogDebug("RemovePermissionFromUserAsync: no UserPermission found for user {UserId}, permission {PermissionId}", userId, permissionId);
                return false;
            }

            _db.UserPermissions.Remove(userPerm);

            try
            {
                await _db.SaveChangesAsync();
                _logger?.LogInformation("Removed permission {PermissionId} from user {UserId}", permissionId, userId);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger?.LogWarning(ex, "Concurrency issue while removing permission {PermissionId} from user {UserId}", permissionId, userId);
                return false;
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogError(ex, "DB error while removing permission {PermissionId} from user {UserId}", permissionId, userId);
                return false;
            }
        }
    }
}
