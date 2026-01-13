using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.Api.Controllers;

[ApiController]
[Route("api/upload-data")]
[Authorize(Roles = "admin,superadmin")]
public class UploadDataController : ControllerBase
{
    private readonly IUploadDataService _service;
    private readonly ILogger<UploadDataController> _logger;

    public UploadDataController(IUploadDataService service, ILogger<UploadDataController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(MaxUploadSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadSizeBytes)]
    [ProducesResponseType(typeof(UploadDataResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadAsync([FromForm] IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "A file is required." });
        }

        if (file.Length > MaxUploadSizeBytes)
        {
            return BadRequest(new { message = "The file exceeds the 10 MB upload limit." });
        }

        var companyId = HttpContext.GetCompanyId();
        _logger.LogInformation("Processing upload file {FileName} ({Size} bytes) for company {CompanyId}", file.FileName, file.Length, companyId);

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _service.ImportAsync(stream, file.FileName, companyId, cancellationToken);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Upload cancelled for company {CompanyId}", companyId);
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Upload cancelled." });
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid upload data for company {CompanyId}", companyId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while importing upload for company {CompanyId}", companyId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Upload failed. Please try again." });
        }
    }

    private const long MaxUploadSizeBytes = 10 * 1024 * 1024; // 10 MB
}
