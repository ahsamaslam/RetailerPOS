using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RetailerMobileApp.Core.Models.Common;

namespace RetailerMobileApp.Infrastructure.Http;

public static class ApiExceptionMapper
{
    public static async Task<ApiResult<T>> HandleAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var payload = await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
            return payload is null
                ? ApiResult<T>.FromError("Response body was empty.")
                : ApiResult<T>.FromData(payload);
        }

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var message = TryExtractError(body) ?? response.ReasonPhrase ?? "Unexpected error";

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            message = "Unauthorized. Please login again.";
        }

        return ApiResult<T>.FromError(message);
    }

    private static string? TryExtractError(string raw)
    {
        try
        {
            using var document = JsonDocument.Parse(raw);
            if (document.RootElement.TryGetProperty("message", out var messageNode))
            {
                return messageNode.GetString();
            }

            if (document.RootElement.TryGetProperty("error", out var errorNode))
            {
                return errorNode.GetString();
            }
        }
        catch (JsonException)
        {
            // ignored on purpose, fallback to raw text
        }

        return string.IsNullOrWhiteSpace(raw) ? null : raw;
    }
}
