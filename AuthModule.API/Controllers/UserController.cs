using AuthModule.API.Dtos;
using AuthModule.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IPermissionService _perm;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(
             IPermissionService perm,
             UserManager<IdentityUser> userManager)
        {
            _perm = perm;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet("permissions")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
            var permissions = await _perm.GetPermissionsForUserAsync(userId);
            return Ok(new UserPermissionDto
            {
                UserId = userId,
                Roles = roles.ToList(),
                Permissions = permissions
            });
        }

    }
}
