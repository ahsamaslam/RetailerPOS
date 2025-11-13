namespace AuthModule.API.Models
{
    public class UserPermission
    {
        public string UserId { get; set; } = default!;
        public int PermissionId { get; set; }
        public bool IsAllowed { get; set; } = true;

        public Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }
        public Permission? Permission { get; set; }

    }
}
