using Microsoft.AspNetCore.Identity;

namespace AuthModule.API.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = default!; // IdentityRole.Id is string
        public int PermissionId { get; set; }


        public IdentityRole? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
