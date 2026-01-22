using Microsoft.Maui.Storage;
using RetailerMobileApp.Core.Constants;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.Core.Models.Auth;

namespace RetailerMobileApp.Infrastructure.Storage;

public class SecureTokenStorageService : ITokenStorageService
{
    public async Task StoreTokensAsync(LoginResultDto tokens, CancellationToken cancellationToken = default)
    {
        await SecureStorage.Default.SetAsync(SecureStorageKeys.AccessToken, tokens.Token).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(tokens.RefreshToken))
        {
            await SecureStorage.Default.SetAsync(SecureStorageKeys.RefreshToken, tokens.RefreshToken).ConfigureAwait(false);
        }
        else
        {
            SecureStorage.Default.Remove(SecureStorageKeys.RefreshToken);
        }
    }

    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        SecureStorage.Default.GetAsync(SecureStorageKeys.AccessToken);

    public Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default) =>
        SecureStorage.Default.GetAsync(SecureStorageKeys.RefreshToken);

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        SecureStorage.Default.Remove(SecureStorageKeys.AccessToken);
        SecureStorage.Default.Remove(SecureStorageKeys.RefreshToken);
        SecureStorage.Default.Remove(SecureStorageKeys.UserId);
        SecureStorage.Default.Remove(SecureStorageKeys.CompanyId);
        SecureStorage.Default.Remove(SecureStorageKeys.BranchId);
        return Task.CompletedTask;
    }
}
