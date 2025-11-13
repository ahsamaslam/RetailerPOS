namespace AuthModule.API.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = default!; // IdentityRole.Id is string
        public int PermissionId { get; set; }


        public Microsoft.AspNetCore.Identity.IdentityRole? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
