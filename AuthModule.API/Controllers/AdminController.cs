using AuthModule.API.Data;
using AuthModule.API.Dtos;
using AuthModule.API.Models;
using AuthModule.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPermissionService _perm;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _db;

        public AdminController(
             IPermissionService perm,
             UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager,
             IConfiguration config,
             ApplicationDbContext db,
             ILogger<AdminController> logger)
        {
            _db = db;
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

        // POST api/admin/users/{userId}/roles
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignRolesToUser(string userId, [FromBody] List<string>? roles)
        {
            // Allow empty list to mean "remove all roles"
            roles ??= new List<string>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            // Normalize and remove empty/whitespace names
            var requested = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Validate role existence
            var missingRoles = new List<string>();
            foreach (var r in requested)
            {
                if (!await _roleManager.RoleExistsAsync(r))
                    missingRoles.Add(r);
            }

            if (missingRoles.Any())
            {
                return NotFound(new { message = "One or more roles not found", missing = missingRoles });
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Determine roles to add and remove
            var toAdd = requested.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();
            var toRemove = currentRoles.Except(requested, StringComparer.OrdinalIgnoreCase).ToList();

            // Remove roles not wanted
            if (toRemove.Any())
            {
                var remResult = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!remResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to remove roles", errors = remResult.Errors });
                }
            }

            // Add new roles
            if (toAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, toAdd);
                if (!addResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to add roles", errors = addResult.Errors });
                }
            }

            // Success - no content
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

            // Read CompanyId from JWT token
            var companyId = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrWhiteSpace(companyId))
                return Unauthorized("CompanyId is missing in token.");

            // Basic uniqueness checks
            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return Conflict("Username already exists.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _userManager.FindByEmailAsync(dto.Email) != null)
                return Conflict("Email already exists.");

            // Determine default and allowed roles from config (fallbacks)
            var defaultRole = _config["Auth:DefaultRole"] ?? "User";
            var allowedRoles = _config.GetSection("Auth:AssignableRoles")?.Get<string[]>() ?? new[] { "user", "manager", "admin" };


            ApplicationUser user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true, // adjust if you want email confirmation flow
                CompanyId = Guid.Parse(companyId)      // ⭐ Assign company from token

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

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // Ensure default role exists (create if missing)
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    var r = await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                    if (!r.Succeeded)
                    {
                        await _userManager.DeleteAsync(user); // cleanup created user
                        return StatusCode(StatusCodes.Status500InternalServerError, "Unable to create default role.");
                    }
                }

                // Assign default role
                var addDefaultRoleRes = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!addDefaultRoleRes.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to assign default role to user.");
                }

                // Get assigned roles (should at least include defaultRole)
                var assignedRoles = await _userManager.GetRolesAsync(user); // list of role names

                // Map role names -> role ids using AspNetRoles table (Role entities)
                var roleEntities = await _db.Roles
                    .Where(r => assignedRoles.Contains(r.Name))
                    .ToListAsync();

                var roleIds = roleEntities.Select(r => r.Id).ToList();

                // Query RolePermissions by RoleId -> fetch Permission entities
                // Assumes RolePermission has RoleId and Permission navigation property loaded or use join
                var rolePermissionEntries = await _db.RolePermissions
                    .Include(rp => rp.Permission)
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .ToListAsync();

                var distinctPermissions = rolePermissionEntries
                    .Select(rp => rp.Permission!)
                    .Where(p => p != null)
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();

                // Insert UserPermission rows for the new user (avoid duplicates)
                var existingUserPermIds = await _db.UserPermissions
                    .Where(up => up.UserId == user.Id)
                    .Select(up => up.PermissionId)
                    .ToListAsync();

                var toAddUserPermissions = distinctPermissions
                    .Where(p => !existingUserPermIds.Contains(p.Id))
                    .Select(p => new UserPermission
                    {
                        UserId = user.Id,
                        PermissionId = p.Id
                    })
                    .ToList();

                if (toAddUserPermissions.Any())
                {
                    _db.UserPermissions.AddRange(toAddUserPermissions);
                    await _db.SaveChangesAsync();
                }

                await tx.CommitAsync();

                // Prepare response
                var permissionsForResponse = distinctPermissions.Select(p => new { p.Id, p.Name }).ToList();

                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.CompanyId,
                    user.picture,
                    Roles = assignedRoles,
                    Permissions = permissionsForResponse
                });
            }
            catch (Exception ex)
            {
                // best-effort cleanup: remove the created user and rollback DB transaction
                try
                {
                    await tx.RollbackAsync();
                }
                catch { /* ignore */ }

                try { await _userManager.DeleteAsync(user); } catch { /* ignore */ }

                // log ex if you have logger: _logger.LogError(ex, "CreateUser failed");
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
        // Update: api/admin/permissions/{id}
        [HttpPut("permissions/{permissionId:int}")]
        public async Task<IActionResult> UpdatePermission([FromBody] PermissionDto permissionDto)
        {
            var removed = await _perm.UpdatePermissionAsync(permissionDto);
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
        [HttpPut("users/{userId}/permissions")]
        public async Task<IActionResult> AssignPermissionToUserEndpoint(string userId, [FromBody] List<int> permissionIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            await _perm.AssignPermissionToUserAsync(userId, permissionIds);
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
