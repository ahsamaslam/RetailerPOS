namespace Retailer.Api.DTOs
{
    public class UserPermissionDto
    {
        public string UserId { get; set; } = string.Empty;      // Id of the logged-in user
        public List<string> Roles { get; set; } = new();        // List of roles assigned to the user
        public List<string> Permissions { get; set; } = new();  // List of permission names assigned to the user
    }
}
