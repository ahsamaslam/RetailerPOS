using System.Security.Claims;

namespace AuthModule.API.Services
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken, System.DateTimeOffset AccessTokenExpiresAt)> IssueTokensAsync(string userId, IEnumerable<Claim> claims);
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(string expiredAccessToken, string refreshToken, string ipAddress);
        Task RevokeRefreshTokenAsync(string tokenHash);
    }
}
