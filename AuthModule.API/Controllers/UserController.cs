using AuthModule.API.Dtos;
using AuthModule.API.Models;
using AuthModule.API.Services;
using Humanizer;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(
             IPermissionService perm,
             UserManager<ApplicationUser> userManager)
        {
            _perm = perm;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<IActionResult> currentUser()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
           
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(user);
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
        [Authorize]
        [HttpPost("CheckCurrentUserPassword")] 
        public async Task<IActionResult> CheckCurrentUserPassword(ChangePasswordDto password)
        {
            string oldPassword = password.CurrentPassword;  
            // Get current user ID from claims
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated.", value=false });

            // Find user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found.", value = false });

            // Check password
            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, oldPassword);

            if (!isPasswordValid)
                return BadRequest(new { message = "Current password is incorrect.", value = false });

            // Password is valid
            return Ok(new { message = " password is correct.", value =true});
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user ID from claims
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found.", value = false });

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                // Aggregate errors to return
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new { message = "Password change failed.", errors, value = false });
            }

            return Ok(new { message = "Password changed successfully.", value = true });
        }

    }
}
