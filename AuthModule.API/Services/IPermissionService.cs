using AuthModule.API.Models;

namespace AuthModule.API.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<Permission> CreatePermissionAsync(string name, string? description = null);
        Task AssignPermissionToRoleAsync(string roleId, int permissionId);
        Task RemovePermissionFromRoleAsync(string roleId, int permissionId);
        Task<IEnumerable<string>> GetPermissionsForRoleAsync(string roleId);
        Task<List<string>> GetPermissionsForUserAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, string permission);
    }

}
