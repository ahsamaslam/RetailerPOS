using AuthModule.API.Models;

namespace AuthModule.API.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> FindByUserIdAndHashAsync(string userId, string tokenHash);
        Task<RefreshToken?> FindByIdAsync(Guid id);
        Task UpdateAsync(RefreshToken token);
        Task RevokeAllForUserAsync(string userId, string reason = "bulk-revoke");
    }
}
