using AuthModule.API.Dtos;
using AuthModule.API.Models;

namespace AuthModule.API.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<Permission> CreatePermissionAsync(string name, string? description = null);
        Task AssignPermissionToRoleAsync(string roleId, int permissionId);
        Task<List<PermissionDto>> GetPermissionsForRoleAsync(string roleId);
        Task<Permission?> GetPermissionAsync(int permissionId);
        Task<List<PermissionDto>> GetPermissionsForUserAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, string permission);
        Task<List<Permission>> GetPermissionsForDefaultUserAsync();
        Task AssignPermissionToUserAsync(string UserId, List<int> permissionIds);
        Task<bool> RemovePermissionFromRoleAsync(string roleId, int permissionId);
        Task<bool> DeletePermissionAsync(int permissionId);
        Task<bool> UpdatePermissionAsync(PermissionDto permissionDto);
        Task<bool> RemovePermissionFromUserAsync(string userId, int permissionId);
    }

}
