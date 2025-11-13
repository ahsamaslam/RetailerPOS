using AuthModule.API.Data;
using AuthModule.API.Repositories;
using AuthModule.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace AuthModule.API.Tests
{
    public class TokenServiceTests
    {
        private static IConfiguration BuildTestConfig()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>
            {
            {"Jwt:SigningKeyBase64", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("VerySecretSigningKey1234567890"))},
            {"Jwt:Issuer", "test-issuer"},
            {"Jwt:Audience", "test-audience"},
            {"Jwt:AccessTokenMinutes", "5"},
            {"Jwt:RefreshTokenDays", "30"},
            {"Jwt:KeyId", "test-keyid"},
            {"Security:HmacKeyBase64", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("HmacKey-which-is-long-and-random-1234567890"))}
            };
            return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        }


        private static ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            return new ApplicationDbContext(options);
        }
        [Fact]
        public async Task IssueTokens_Then_Refresh_RotatesTokens()
        {
            var config = BuildTestConfig();
            var db = CreateInMemoryDb();
            var repo = new RefreshTokenRepository(db);
            var hasher = new TokenHasher(config["Security:HmacKeyBase64"]!);
            var svc = new TokenService(config, hasher, repo);


            var userId = "user-1";
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId), new Claim(ClaimTypes.Email, "a@b.com") };


            var issued = await svc.IssueTokensAsync(userId, claims);
            Assert.False(string.IsNullOrEmpty(issued.AccessToken));
            Assert.False(string.IsNullOrEmpty(issued.RefreshToken));


            var refreshed = await svc.RefreshAsync(issued.AccessToken, issued.RefreshToken, "127.0.0.1");
            Assert.False(string.IsNullOrEmpty(refreshed.AccessToken));
            Assert.False(string.IsNullOrEmpty(refreshed.RefreshToken));


            var oldHash = hasher.Hash(issued.RefreshToken);
            var oldEntity = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == oldHash);
            Assert.NotNull(oldEntity);
            Assert.NotNull(oldEntity.RevokedAt);
            Assert.NotNull(oldEntity.ReplacedByTokenId);


            var newHash = hasher.Hash(refreshed.RefreshToken);
            var newEntity = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == newHash);
            Assert.NotNull(newEntity);
            Assert.Null(newEntity.RevokedAt);
        }
        [Fact]
        public async Task ReuseRevokedRefreshToken_TriggersBulkRevoke_AndThrows()
        {
            var config = BuildTestConfig();
            var db = CreateInMemoryDb();
            var repo = new RefreshTokenRepository(db);
            var hasher = new TokenHasher(config["Security:HmacKeyBase64"]!);
            var svc = new TokenService(config, hasher, repo);


            var userId = "user-2";
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };


            var issued = await svc.IssueTokensAsync(userId, claims);


            var refreshed = await svc.RefreshAsync(issued.AccessToken, issued.RefreshToken, "127.0.0.1");


            var ex = await Assert.ThrowsAsync<Microsoft.IdentityModel.Tokens.SecurityTokenException>(async () =>
            {
                await svc.RefreshAsync(issued.AccessToken, issued.RefreshToken, "127.0.0.1");
            });


            Assert.Contains("revoked", ex.Message, StringComparison.InvariantCultureIgnoreCase);


            var tokens = await db.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
            Assert.All(tokens, t => Assert.NotNull(t.RevokedAt));
        }
    }
}
