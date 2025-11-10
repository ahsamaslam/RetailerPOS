using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Retailer.POS.Api.Repositories;
using Retailer.POS.API.UnitOfWork;

namespace Retailer.POS.Api.Services;
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _cfg;

    public AuthService(IUnitOfWork uow, IConfiguration cfg)
    {
        _uow = uow;
        _cfg = cfg;
    }

    // Simple validation: compares SHA256(password) with stored PasswordHash.
    public async Task<string?> ValidateAndCreateTokenAsync(string username, string password)
    {
        var db = (_uow as UnitOfWork)!.GetDbContext();
        var login = await db.Logins.FirstOrDefaultAsync(l => l.Username == username && l.IsActive);
        if (login == null) return null;

        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha.ComputeHash(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        if (!string.Equals(hash, login.PasswordHash, StringComparison.OrdinalIgnoreCase))
            return null;

        var jwtSection = _cfg.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key"));
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim(ClaimTypes.NameIdentifier, login.Id.ToString())
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpiresMinutes")),
            Issuer = jwtSection.GetValue<string>("Issuer"),
            Audience = jwtSection.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
