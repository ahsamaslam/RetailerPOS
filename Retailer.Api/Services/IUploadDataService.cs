using Retailer.POS.Api.DTOs;

namespace Retailer.POS.Api.Services;

public interface IUploadDataService
{
    Task<UploadDataResultDto> ImportAsync(Stream stream, string fileName, Guid companyId, CancellationToken cancellationToken);
}
