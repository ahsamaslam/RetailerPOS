using AuthModule.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid username or password");

            // ============================
            //  BASIC USER CLAIMS
            // ============================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("sub", user.Id)
            };

            // ============================
            //   ROLE CLAIMS
            // ============================
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // ============================
            //   PERMISSION CLAIMS
            // ============================
           
            var permissions = await _permissionService.GetPermissionsForUserAsync(user.Id);

            foreach (var p in permissions.Distinct())
                claims.Add(new Claim("permission", p));

            // ============================
            //   GENERATE TOKEN
            // ============================
            var token = GenerateJwtToken(claims);

            return Ok(new
            {
                token,
                expiresIn = _config["Jwt:ExpiresInMinutes"],
                roles,
                permissions
            });
        }
        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:SigningKeyBase64"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record LoginDto(string UserName, string Password);
}
