using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RetailerMobileApp.Core.Interfaces;
using RetailerMobileApp.Core.Models.Sales;

namespace RetailerMobileApp.Infrastructure.Http;

public class SalesApiClient : ISalesApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SalesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SalesMasterDto>> GetSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var start = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var end = endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var response = await _httpClient.GetAsync($"api/Sales/GetAllDateWise/{start}/{end}", cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<List<SalesMasterDto>>(_serializerOptions, cancellationToken).ConfigureAwait(false)
               ?? new List<SalesMasterDto>();
    }

    public async Task<SalesMasterDto> GetSaleAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/Sales/{id}", cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<SalesMasterDto>(_serializerOptions, cancellationToken).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Empty sales response received from server.");
    }

    public async Task<SalesMasterDto> CreateSaleAsync(SalesMasterDto sale, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Sales", sale, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<SalesMasterDto>(_serializerOptions, cancellationToken).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Empty sales response received from server.");
    }

    public async Task UpdateSaleAsync(int id, SalesMasterDto sale, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Sales/{id}", sale, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    public async Task DeleteSaleAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/Sales/{id}", cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var message = string.IsNullOrWhiteSpace(content)
            ? $"Request failed with status code {(int)response.StatusCode}."
            : content;

        throw new InvalidOperationException(message);
    }
}
