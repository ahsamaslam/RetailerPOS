using Retailer.POS.Api.DTOs;

namespace Retailer.POS.Api.Services;

public interface IUploadStockService
{
    Task<UploadStockResultDto> ImportAsync(Stream stream, string fileName, Guid companyId, CancellationToken cancellationToken);
}
