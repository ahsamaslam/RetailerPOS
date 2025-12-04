using AuthModule.API.Dtos;
using AuthModule.API.Models;
using AuthModule.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
   // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPermissionService _perm;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminController> _logger;
        public AdminController(
             IPermissionService perm,
             UserManager<IdentityUser> userManager,
             RoleManager<IdentityRole> roleManager,
             IConfiguration config,
             ILogger<AdminController> logger)
        {
            _perm = perm;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;
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
            if (dto == null) return BadRequest("Request body required.");
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            // Basic uniqueness checks
            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return Conflict("Username already exists.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _userManager.FindByEmailAsync(dto.Email) != null)
                return Conflict("Email already exists.");

            // Determine default and allowed roles from config (fallbacks)
            var defaultRole = _config["Auth:DefaultRole"] ?? "User";
            var allowedRoles = _config.GetSection("Auth:AssignableRoles")?.Get<string[]>() ?? new[] { "User", "Manager", "Admin" };


            IdentityUser user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true // adjust if you want email confirmation flow
            };

            // Create user via UserManager (hashes password, etc.)
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                // no DB writes except Identity user creation (handled by UserManager)
                // return detailed errors
                var errors = createResult.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new { errors });
            }

            try
            {
                // Ensure default/target role exists (create if missing)
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    var r = await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                    if (!r.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create default role.");
                    }
                }

                // Assign default role first (always)
                var addDefaultRoleRes = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!addDefaultRoleRes.Succeeded)
                {
                    // cleanup and rollback
                    await _userManager.DeleteAsync(user);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to assign default role to user.");
                }

                  // _perm.gete


               var permissions = await _perm.GetPermissionsForDefaultUserAsync();


              
                foreach (var perm in permissions)
                {

                    await _perm.AssignPermissionToUserAsync(user.Id, perm.Id);
                     
                }

                // Build response
                var assignedRoles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = assignedRoles,
                    permissions
                });
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error creating user {UserName}", dto.UserName);
                // Try cleanup: delete the identity user (if created)
                try { await _userManager.DeleteAsync(user); } catch { /* ignore */ }
               return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred creating the user.");
            }
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
        // ----------------- PERMISSIONS endpoints -----------------

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

        // GET: api/admin/permissions
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var perms = await _perm.GetAllPermissionsAsync();
            // return minimal view model expected by your Razor page
            var vm = perms.Select(p => new PermissionDto(p.Id,p.Name,p.Description ?? "")).ToList();
            return Ok(vm);
        }

        // GET: api/admin/permissions/{id}
        [HttpGet("permissions/{permissionId:int}")]
        public async Task<IActionResult> GetPermission(int permissionId)
        {
            var p = await _perm.GetPermissionAsync(permissionId);
            if (p == null) return NotFound();
            return Ok(new PermissionDto(p.Id, p.Name, p.Description ?? ""));
        }

        // DELETE: api/admin/permissions/{id}
        [HttpDelete("permissions/{permissionId:int}")]
        public async Task<IActionResult> DeletePermission(int permissionId)
        {
            var removed = await _perm.DeletePermissionAsync(permissionId);
            if (!removed) return NotFound();
            return NoContent();
        }

        // GET: api/admin/roles/{roleId}/permissions
        [HttpGet("roles/{roleId}/permissions")]
        public async Task<IActionResult> GetPermissionsForRole(string roleId)
        {
            // validate role exists?
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Role not found");

            var perms = await _perm.GetPermissionsForRoleAsync(roleId);
            return Ok(perms);
        }

        // DELETE: api/admin/roles/{roleId}/permissions/{permissionId}
        [HttpDelete("roles/{roleId}/permissions/{permissionId:int}")]
        public async Task<IActionResult> RemovePermissionFromRole(string roleId, int permissionId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Role not found");

            var removed = await _perm.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!removed) return NotFound();

            return NoContent();
        }

        // GET: api/admin/users/{userId}/permissions
        [HttpGet("users/{userId}/permissions")]
        public async Task<IActionResult> GetPermissionsForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var perms = await _perm.GetPermissionsForUserAsync(userId);
            return Ok(perms);
        }

        // POST: api/admin/users/{userId}/permissions/{permissionId}
        [HttpPost("users/{userId}/permissions/{permissionId:int}")]
        public async Task<IActionResult> AssignPermissionToUserEndpoint(string userId, int permissionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            await _perm.AssignPermissionToUserAsync(userId, permissionId);
            return NoContent();
        }

        // DELETE: api/admin/users/{userId}/permissions/{permissionId}
        [HttpDelete("users/{userId}/permissions/{permissionId:int}")]
        public async Task<IActionResult> RemovePermissionFromUserEndpoint(string userId, int permissionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var removed = await _perm.RemovePermissionFromUserAsync(userId, permissionId);
            if (!removed) return NotFound();

            return NoContent();
        }
    }


    // DTO for creating a user
    public record CreateUserDto(string UserName, string Email, string Password);
    public record CreatePermissionDto(string Name, string? Description);
    public record CreateRoleDto(string Name);

}
