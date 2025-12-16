using AuthModule.API.Dtos;
using AuthModule.API.Models;
using AuthModule.API.Services;
using Azure.Core;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.API.Controllers
{
    [Route("api/authuser")]
    [ApiController]
    [Authorize]
    public class AuthUserController : ControllerBase
    {
        private readonly IPermissionService _perm;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly string serverPath;

        public AuthUserController(
             IPermissionService perm,
             UserManager<ApplicationUser> userManager,
             IWebHostEnvironment env,
              IHttpContextAccessor httpContextAccessor)
        {
            _perm = perm;
            _userManager = userManager;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            var request = _httpContextAccessor.HttpContext?.Request;

              serverPath =
                $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }

        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
           
            return Ok(new UserDto()
            {
                CompanyId = user.CompanyId,
                Email = user.Email,
                Id = userId,
                picture = string.IsNullOrEmpty(user.picture)?"": serverPath + user.picture,
                UserName = user.UserName
            });
        }
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
                Permissions = permissions.Select(x => x.Name).ToList()
            });
        }
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
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            // Update basic fields
            user.UserName = dto.UserName;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            // Handle picture upload
            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "users");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Picture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Picture.CopyToAsync(stream);

                user.picture = $"/uploads/users/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.picture
            });
        }

    }
}
