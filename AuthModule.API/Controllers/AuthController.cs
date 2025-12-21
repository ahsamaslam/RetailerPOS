using AuthModule.API.Data;
using AuthModule.API.Models;
using AuthModule.API.Services;
using Azure.Core;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private   string serverPath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;


        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService,
            IConfiguration config,
            ApplicationDbContext db,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
            _config = config; 
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            var request = _httpContextAccessor.HttpContext?.Request;

            serverPath =
              $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _db.Users
                            .Include(u => u.Company)
                            .FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid username or password");

            // ============================
            //  BASIC USER CLAIMS
            // ============================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("sub", user.Id),
                new Claim("picture", !string.IsNullOrEmpty(user.picture)?  serverPath+ user.picture?.ToString() : string.Empty) // FIX: null-safe

            };

            // ============================
            //   ROLE CLAIMS
            // ============================
            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Contains("superadmin", StringComparer.OrdinalIgnoreCase);
            if (!isSuperAdmin)
            {
                if (!user.CompanyId.HasValue)
                    return BadRequest("User is not assigned to a company.");

                claims.Add(new Claim("companyId", user.CompanyId.Value.ToString()));
                claims.Add(new Claim("companyName", user.Company?.Name ?? string.Empty));
            }
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // ============================
            //   PERMISSION CLAIMS
            // ============================
           
            var permissions = await _permissionService.GetPermissionsForUserAsync(user.Id);

            foreach (var p in permissions.Distinct())
                claims.Add(new Claim("permission", p.Name));

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

    public record LoginRequestDto(string UserName, string Password);
}
