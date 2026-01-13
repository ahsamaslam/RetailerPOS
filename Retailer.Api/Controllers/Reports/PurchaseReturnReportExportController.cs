using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services.Reports.Interface;

namespace Retailer.Api.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseReturnReportExportController : ControllerBase
    {
        private readonly IPurchaseReturnReportExportService _service;

        public PurchaseReturnReportExportController(IPurchaseReturnReportExportService service)
        {
            _service = service;
        }

        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet("date-wise")]
        public async Task<IActionResult> DateWiseAsync([FromQuery] string export, [FromQuery] DateTime sdate, [FromQuery] DateTime edate)
        {
            var bytes = await _service.GeneratePurchaseReturnReportAsync(sdate, edate, CompanyId, export);
            return File(bytes, ResolveContentType(export), BuildFileName("PurchaseReturnReport", export));
        }

        [HttpGet("vendor-wise")]
        public async Task<IActionResult> VendorWiseAsync([FromQuery] int vendorId, [FromQuery] string export, [FromQuery] DateTime sdate, [FromQuery] DateTime edate)
        {
            var bytes = await _service.GenerateVendorPurchaseReturnReportAsync(vendorId, sdate, edate, CompanyId, export);
            return File(bytes, ResolveContentType(export), BuildFileName("PurchaseReturnReport", export));
        }

        [HttpGet("item-wise")]
        public async Task<IActionResult> ItemWiseAsync([FromQuery] int itemId, [FromQuery] string export, [FromQuery] DateTime sdate, [FromQuery] DateTime edate)
        {
            var bytes = await _service.GenerateItemPurchaseReturnReportAsync(itemId, sdate, edate, CompanyId, export);
            return File(bytes, ResolveContentType(export), BuildFileName("PurchaseReturnItemReport", export));
        }

        private static string ResolveContentType(string export) => export.ToLowerInvariant() switch
        {
            "pdf" => "application/pdf",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "word" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        private static string BuildFileName(string prefix, string export)
        {
            var extension = export.ToLowerInvariant() switch
            {
                "excel" => "xlsx",
                "word" => "docx",
                _ => "pdf"
            };

            return $"{prefix}_{DateTime.Now:yyyyMMdd}.{extension}";
        }
    }
}
