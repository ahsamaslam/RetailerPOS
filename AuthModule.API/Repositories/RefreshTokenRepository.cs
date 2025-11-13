using AuthModule.API.Data;
using AuthModule.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;
        public RefreshTokenRepository(ApplicationDbContext db) => _db = db;


        public async Task AddAsync(RefreshToken token)
        {
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
        }


        public async Task<RefreshToken?> FindByUserIdAndHashAsync(string userId, string tokenHash)
        {
            return await _db.RefreshTokens
            .Where(x => x.UserId == userId && x.TokenHash == tokenHash)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
        }


        public async Task<RefreshToken?> FindByIdAsync(Guid id) => await _db.RefreshTokens.FindAsync(id);


        public async Task UpdateAsync(RefreshToken token)
        {
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
        }


        public async Task RevokeAllForUserAsync(string userId, string reason = "bulk-revoke")
        {
            var tokens = await _db.RefreshTokens.Where(x => x.UserId == userId && x.RevokedAt == null).ToListAsync();
            foreach (var t in tokens) t.RevokedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
