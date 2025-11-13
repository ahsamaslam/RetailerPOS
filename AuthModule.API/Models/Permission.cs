namespace AuthModule.API.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;


        public ICollection<RolePermission>? RolePermissions { get; set; }
        public ICollection<UserPermission>? UserPermissions { get; set; }
    }
}
