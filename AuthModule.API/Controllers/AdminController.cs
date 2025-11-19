using AuthModule.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPermissionService _perm;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(
             IPermissionService perm,
             UserManager<IdentityUser> userManager,
             RoleManager<IdentityRole> roleManager)
        {
            _perm = perm;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPost("permissions")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            var p = await _perm.CreatePermissionAsync(dto.Name, dto.Description);
            return Ok(p);
        }


        [HttpPost("roles/{roleId}/permissions/{permissionId}")]
        public async Task<IActionResult> AssignToRole(string roleId, int permissionId)
        {
            await _perm.AssignPermissionToRoleAsync(roleId, permissionId);
            return NoContent();
        }
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.Name))
                return Conflict("Role already exists");

            var role = new IdentityRole(dto.Name);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(role);
        }

        [HttpPost("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("Role not found");

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("Role not found");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
        // ---------------- USERS ----------------

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users.Select(u => new { u.Id, u.UserName, u.Email }));
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            // Check if user already exists by username or email
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return Conflict("Username already exists");

            existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Conflict("Email already exists");

            // Create the new user
            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true // Optional: set to false if email confirmation is required
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { user.Id, user.UserName, user.Email });
        }
        [HttpGet("roles/names")]
        public IActionResult GetAllRoleNames() =>
        Ok(_roleManager.Roles.Select(r => r.Name).ToList());

        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles
                .Select(r => new { r.Id, r.Name })
                .ToList();
            return Ok(roles);
        }
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var perms = await _perm.GetAllPermissionsAsync(); // Implement this in IPermissionService
            return Ok(perms.Select(p => new { p.Id, p.Name, p.Description }));
        }
    }


    // DTO for creating a user
    public record CreateUserDto(string UserName, string Email, string Password);
    public record CreatePermissionDto(string Name, string? Description);
    public record CreateRoleDto(string Name);

}
