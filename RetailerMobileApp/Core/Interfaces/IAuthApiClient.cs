using RetailerMobileApp.Core.Models.Auth;

namespace RetailerMobileApp.Core.Interfaces;

public interface IAuthApiClient
{
    Task<LoginResultDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
}
