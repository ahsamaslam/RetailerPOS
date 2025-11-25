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
                roles 
            });
        }
        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            // read signing key: prefer base64 entry, otherwise use plain text key
            var base64Key = _config["Jwt:SigningKeyBase64"];
            byte[] keyBytes;
            if (!string.IsNullOrWhiteSpace(base64Key))
            {
                try
                {
                    keyBytes = Convert.FromBase64String(base64Key);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException("Jwt:SigningKeyBase64 must be a valid base64 string.", ex);
                }
            }
            else
            {
                var plainKey = _config["Jwt:Key"];
                if (string.IsNullOrEmpty(plainKey))
                    throw new InvalidOperationException("No signing key found. Set Jwt:SigningKeyBase64 or Jwt:Key in configuration.");

                keyBytes = Encoding.UTF8.GetBytes(plainKey);
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing in configuration.");

            // read audiences: support either an array (Audiences) or single (Audience)
            var audiences = _config.GetSection("Jwt:Audiences").Get<string[]>();
           
            // expiry
            var expiresMinutesText = _config["Jwt:ExpiresInMinutes"];
            if (!double.TryParse(expiresMinutesText, out var expiresMinutes))
                expiresMinutes = 60; // default

            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            JwtSecurityToken jwt;
            if (audiences == null || audiences.Length == 0)
            {
                // single/no audience case: use null audience (no 'aud') or you can set single audience via constructor
                jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: null,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: creds
                );
            }
            else if (audiences.Length == 1)
            {
                // single audience: use constructor audience param for simplicity
                jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audiences[0],
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: creds
                );
            }
            else
            {
                // multiple audiences: create token without audience then inject aud array into payload
                jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: null,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: creds
                );

                // set aud as array in payload
                jwt.Payload["aud"] = audiences;
            }

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public record LoginDto(string UserName, string Password);
}
