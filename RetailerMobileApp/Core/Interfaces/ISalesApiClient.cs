using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RetailerMobileApp.Core.Models.Sales;

namespace RetailerMobileApp.Core.Interfaces;

public interface ISalesApiClient
{
    Task<IReadOnlyList<SalesMasterDto>> GetSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<SalesMasterDto> GetSaleAsync(int id, CancellationToken cancellationToken = default);
    Task<SalesMasterDto> CreateSaleAsync(SalesMasterDto sale, CancellationToken cancellationToken = default);
    Task UpdateSaleAsync(int id, SalesMasterDto sale, CancellationToken cancellationToken = default);
    Task DeleteSaleAsync(int id, CancellationToken cancellationToken = default);
}
