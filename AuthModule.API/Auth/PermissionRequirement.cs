// Auth/PermissionRequirement.cs
using Microsoft.AspNetCore.Authorization;

namespace AuthModule.API.Auth
{
    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }
}
