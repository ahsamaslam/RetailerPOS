using AuthModule.API.Models;

namespace AuthModule.API.Services
{
    public interface IPermissionService
    {
        Task<Permission> CreatePermissionAsync(string name, string? description = null);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task AssignPermissionToRoleAsync(string roleId, int permissionId);
        Task RemovePermissionFromRoleAsync(string roleId, int permissionId);
        Task<IEnumerable<string>> GetPermissionsForUserAsync(string userId);
        Task<IEnumerable<string>> GetPermissionsForRoleAsync(string roleId);
    }
}
