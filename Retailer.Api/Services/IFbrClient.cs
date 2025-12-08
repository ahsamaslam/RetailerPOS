using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.Api.Services
{
    public record FbrResult(bool Success, string? ExternalId = null, string? Message = null);

    public interface IFbrClient
    {
        Task<FbrResult> SendInvoiceAsync(CompanyDto company, SalesMaster sale,Customer customer);

    }
}
