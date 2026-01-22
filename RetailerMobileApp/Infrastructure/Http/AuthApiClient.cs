using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.Core.Models.Auth;

namespace RetailerMobileApp.Infrastructure.Http;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResultDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto, cancellationToken).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content
                .ReadFromJsonAsync<AuthLoginResponse>(_serializerOptions, cancellationToken)
                .ConfigureAwait(false);

            if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
            {
                throw new InvalidOperationException("Empty response from authentication service.");
            }

            var roles = authResponse.Roles ?? Array.Empty<string>();
            var expiresIn = ParseExpiresIn(authResponse.ExpiresIn);

            return new LoginResultDto(authResponse.Token, expiresIn, roles, authResponse.RefreshToken);
        }

        var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(error))
        {
            error = "Unable to sign in. Please try again.";
        }

        throw new InvalidOperationException(error);
    }

    private static double ParseExpiresIn(string? value)
    {
        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return 0d;
    }

    private sealed record AuthLoginResponse(string? Token, string? ExpiresIn, string[]? Roles, string? RefreshToken);
}
