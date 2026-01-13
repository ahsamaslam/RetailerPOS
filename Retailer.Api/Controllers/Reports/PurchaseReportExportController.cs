using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services.Reports.Interface;
using Retailer.POS.Api.Repositories;
using System.Composition;

namespace Retailer.Api.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseReportExportController : ControllerBase
    {
        private readonly IPurchaseReportExportService _service;

       public  PurchaseReportExportController(IPurchaseReportExportService service) 
        {
            _service = service;
        
        
        }
        private Guid CompanyId => HttpContext.GetCompanyId();
        [HttpGet("date-wise")]
        public async Task<IActionResult> DateWisePurchaseAsync(
         [FromQuery] string export,
         [FromQuery] DateTime sdate,
         [FromQuery] DateTime edate)
        {
            {
                var bytes = await _service.GeneratePurchaseReportAsync(sdate, edate, CompanyId, export);

                var contentType = export.ToLower() switch
                {
                    "pdf" => "application/pdf",
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "word" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                var fileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd}.{(export == "excel" ? "xlsx" : export == "word" ? "docx" : "pdf")}";

                return File(bytes, contentType, fileName);
            }

        }
        [HttpGet("vendor-wise")]
        public async Task<IActionResult> VendorWisePurchaseAsync(
        [FromQuery] int vendorID,
        [FromQuery] string export,
        [FromQuery] DateTime sdate,
        [FromQuery] DateTime edate)
        {
            {
                var bytes = await _service.GenerateVendorPurchaseReportAsync(vendorID,sdate, edate, CompanyId, export);

                var contentType = export.ToLower() switch
                {
                    "pdf" => "application/pdf",
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "word" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                var fileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd}.{(export == "excel" ? "xlsx" : export == "word" ? "docx" : "pdf")}";

                return File(bytes, contentType, fileName);
            }

        }
        [HttpGet("item-wise")]
        public async Task<IActionResult> ItemWisePurchaseAsync(
        [FromQuery] int itemID,
        [FromQuery] string export,
        [FromQuery] DateTime sdate,
        [FromQuery] DateTime edate)
        {
            {
                var bytes = await _service.GenerateItemPurchaseReportAsync(itemID, sdate, edate, CompanyId, export);

                var contentType = export.ToLower() switch
                {
                    "pdf" => "application/pdf",
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "word" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                var fileName = $"PurchaseItemReport_{DateTime.Now:yyyyMMdd}.{(export == "excel" ? "xlsx" : export == "word" ? "docx" : "pdf")}";

                return File(bytes, contentType, fileName);
            }

        }

    }
}
