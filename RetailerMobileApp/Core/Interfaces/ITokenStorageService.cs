using RetailerMobileApp.Core.Models.Auth;

namespace RetailerMobileApp.Core.Interfaces;

public interface ITokenStorageService
{
    Task StoreTokensAsync(LoginResultDto tokens, CancellationToken cancellationToken = default);
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
