using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthModule.API.Data;
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

        public PermissionService(ApplicationDbContext db, IMemoryCache cache)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<Permission> CreatePermissionAsync(string name, string? description = null)
        {
            var perm = new Permission { Name = name, Description = description };
            _db.Permissions.Add(perm);
            await _db.SaveChangesAsync();
            _cache.Remove("permissions:all");
            return perm;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _cache.GetOrCreateAsync("permissions:all", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _db.Permissions.AsNoTracking().ToListAsync();
            });
        }

        public async Task AssignPermissionToRoleAsync(string roleId, int permissionId)
        {
            if (await _db.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)) return;
            _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            await _db.SaveChangesAsync();
            _cache.Remove($"permissions:role:{roleId}");
            _cache.Remove("permissions:all");
        }

        public async Task RemovePermissionFromRoleAsync(string roleId, int permissionId)
        {
            var existing = await _db.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (existing == null) return;
            _db.RolePermissions.Remove(existing);
            await _db.SaveChangesAsync();
            _cache.Remove($"permissions:role:{roleId}");
            _cache.Remove("permissions:all");
        }

        public async Task<IEnumerable<string>> GetPermissionsForRoleAsync(string roleId)
        {
            var key = $"permissions:role:{roleId}";
            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _db.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.Permission!.Name)
                    .ToListAsync();
            });
        }

        public async Task<List<string>> GetPermissionsForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return new List<string>();

            var key = $"permissions:user:{userId}";
            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                // Get role ids for this user (Identity stores in AspNetUserRoles)
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
                    .Select(up => new { up.Permission!.Name, up.IsAllowed })
                    .ToListAsync();

                var result = new HashSet<string>(rolePerms, StringComparer.OrdinalIgnoreCase);

                foreach (var up in userPerms)
                {
                    if (up.IsAllowed) result.Add(up.Name);
                    else result.RemoveWhere(n => string.Equals(n, up.Name, StringComparison.OrdinalIgnoreCase));
                }

                return result.ToList();
            });
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
    }
}
