using AuthModule.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;


        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ITokenService tokenService, IPermissionService permissionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _permissionService = permissionService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();


            var res = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!res.Succeeded) return Unauthorized();


            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Email, user.Email ?? "") };
            var perms = await _permissionService.GetPermissionsForUserAsync(user.Id);
            var permClaims = perms.Select(p => new Claim("permission", p));


            var allClaims = claims.Concat(permClaims);


            var tokens = await _tokenService.IssueTokensAsync(user.Id, allClaims);
            return Ok(new { tokens.AccessToken, tokens.RefreshToken, expiresAt = tokens.AccessTokenExpiresAt });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest dto)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var (access, refresh) = await _tokenService.RefreshAsync(dto.AccessToken, dto.RefreshToken, ip);
                return Ok(new { access, refresh });
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
    public record LoginDto(string Email, string Password);
    public record RefreshRequest(string AccessToken, string RefreshToken);
}
