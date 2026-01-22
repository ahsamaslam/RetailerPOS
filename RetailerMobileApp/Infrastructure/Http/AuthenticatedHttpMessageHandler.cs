using System.Net.Http.Headers;
using RetailerMobileApp.Core.Interfaces;

namespace RetailerMobileApp.Infrastructure.Http;

public class AuthenticatedHttpMessageHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;

    public AuthenticatedHttpMessageHandler(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
