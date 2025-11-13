using AuthModule.API.Models;
using AuthModule.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthModule.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly TokenHasher _tokenHasher;
        private readonly IRefreshTokenRepository _repo;
        private readonly JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();


        public TokenService(IConfiguration config, TokenHasher tokenHasher, IRefreshTokenRepository repo)
        {
            _config = config;
            _tokenHasher = tokenHasher;
            _repo = repo;
        }


        private SigningCredentials GetSigningCredentials()
        {
            var secretBase64 = _config["Jwt:SigningKeyBase64"] ?? throw new InvalidOperationException("Jwt:SigningKeyBase64 missing");
            var keyBytes = Convert.FromBase64String(secretBase64);
            var key = new SymmetricSecurityKey(keyBytes) { KeyId = _config["Jwt:KeyId"] };
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }


        public async Task<(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAt)> IssueTokensAsync(string userId, IEnumerable<Claim> claims)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "15");
            var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30");


            var signing = GetSigningCredentials();
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(accessMinutes);


            var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: signing);


            var accessToken = _jwtHandler.WriteToken(jwt);


            var refreshTokenPlain = GenerateSecureToken(64);
            var hashed = _tokenHasher.Hash(refreshTokenPlain);


            var refreshEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = hashed,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(refreshDays)
            };


            await _repo.AddAsync(refreshEntity);


            return (accessToken, refreshTokenPlain, DateTimeOffset.UtcNow.AddMinutes(accessMinutes));
        }


        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string expiredAccessToken, string incomingRefreshToken, string ipAddress)
        {
            var principal = GetPrincipalFromExpiredToken(expiredAccessToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new SecurityTokenException("Invalid access token: missing subject");


            var hashedIncoming = _tokenHasher.Hash(incomingRefreshToken);
            var stored = await _repo.FindByUserIdAndHashAsync(userId, hashedIncoming);


            if (stored == null) throw new SecurityTokenException("Invalid refresh token");


            if (stored.RevokedAt != null)
            {
                await _repo.RevokeAllForUserAsync(userId);
                throw new SecurityTokenException("Refresh token was revoked. Possible token theft - all sessions revoked.");
            }


            if (stored.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                stored.RevokedAt = DateTimeOffset.UtcNow;
                await _repo.UpdateAsync(stored);
                throw new SecurityTokenException("Refresh token expired");
            }


            var newPlain = GenerateSecureToken(64);
            var newHash = _tokenHasher.Hash(newPlain);
            var newEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = stored.UserId,
                TokenHash = newHash,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30")),
                IpAddress = ipAddress
            };


            stored.RevokedAt = DateTimeOffset.UtcNow;
            stored.ReplacedByTokenId = newEntity.Id;


            await _repo.UpdateAsync(stored);
            await _repo.AddAsync(newEntity);


            var claims = principal.Claims.Where(c => c.Type != JwtRegisteredClaimNames.Exp && c.Type != JwtRegisteredClaimNames.Nbf);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "15");


            var signing = GetSigningCredentials();
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(accessMinutes);


            var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: signing);


            var newAccess = _jwtHandler.WriteToken(jwt);


            return (newAccess, newPlain);
        }
        public Task RevokeRefreshTokenAsync(string tokenHash)
        {
            throw new NotImplementedException();
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secretBase64 = _config["Jwt:SigningKeyBase64"]!;
            var keyBytes = Convert.FromBase64String(secretBase64);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = false
            };


            try
            {
                var principal = _jwtHandler.ValidateToken(token, validationParameters, out var rawValidatedToken);
                if (!(rawValidatedToken is JwtSecurityToken jwt) ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm");
                }
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }


        private static string GenerateSecureToken(int sizeBytes)
        {
            var bytes = new byte[sizeBytes];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}