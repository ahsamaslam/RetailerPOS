using AuthModule.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthModule.API.Auth
{
    public class DbPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;
        private readonly IPermissionService _permissionService;


        public DbPolicyProvider(IOptions<AuthorizationOptions> options, IPermissionService permissionService)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);
            _permissionService = permissionService;
        }


        public Task<AuthorizationPolicy?> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();


        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("perm:", System.StringComparison.OrdinalIgnoreCase))
            {
                var perm = policyName.Substring("perm:".Length);
                var exists = (await _permissionService.GetAllPermissionsAsync()).Any(p => p.Name == perm);
                if (!exists) return null;


                var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(perm))
                .Build();
                return policy;
            }


            return await _fallback.GetPolicyAsync(policyName);
        }
    }
}
