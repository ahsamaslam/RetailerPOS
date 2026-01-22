using System;

namespace RetailerMobileApp.Core.Options;

public class ApiEndpointsOptions
{
    public const string SectionName = "ApiEndpoints";

    public string AuthModuleBaseUrl { get; init; } = string.Empty;
    public string RetailerApiBaseUrl { get; init; } = string.Empty;

    public Uri AuthModuleBaseAddress { get; private set; } = default!;
    public Uri RetailerApiBaseAddress { get; private set; } = default!;

    public void Validate()
    {
        AuthModuleBaseAddress = EnsureAbsoluteUri(AuthModuleBaseUrl, nameof(AuthModuleBaseUrl));
        RetailerApiBaseAddress = EnsureAbsoluteUri(RetailerApiBaseUrl, nameof(RetailerApiBaseUrl));
    }

    private static Uri EnsureAbsoluteUri(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{SectionName}:{propertyName}' is required.");
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Configuration value '{SectionName}:{propertyName}' must be a valid absolute URL.");
        }

        return uri;
    }
}
