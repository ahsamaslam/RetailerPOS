using Microsoft.AspNetCore.Authorization;

namespace AuthModule.API.Auth
{
    /// <summary>
    /// Put this on controllers/actions or razor page handlers:
    /// [RequiresPermission("Employees.Create")]
    /// </summary>
    public class RequiresPermissionAttribute : AuthorizeAttribute
    {
        public RequiresPermissionAttribute(string permission)
        {
            Policy = PermissionPolicyName;
            Permission = permission;
        }

        public const string PermissionPolicyName = "Permission";
        public string Permission { get; }
    }
}
