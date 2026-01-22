using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.Helpers;

namespace Retailer.Web.Pages.Admin
{
    public class UploadDataModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<UploadDataModel> _logger;
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public string SampleTemplateUrl => "/templates/upload-data-template.csv";

        [BindProperty]
        public IFormFile? UploadFile { get; set; }
         

        [BindProperty(SupportsGet = true)]

        public string? LoaderType { get; set; }

        public UploadDataModel(IApiClient apiClient, ILogger<UploadDataModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnGetDownloadFile()
        {
            if (string.IsNullOrWhiteSpace(LoaderType))
                return BadRequest("Loader type is required.");

            var loader = LoaderType.Trim();
            if (string.Equals(loader, "Item", StringComparison.OrdinalIgnoreCase))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "upload-data-template.csv");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Template not found.");
                }

                var bytes = System.IO.File.ReadAllBytes(filePath);
                return File(bytes, "text/csv", "upload-data-template.csv");
            }

            if (string.Equals(loader, "Stock", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = await _apiClient.ItemCsvExport();
                return File(bytes, "text/csv", "stock-upload-template.csv");
            }

            return BadRequest("Invalid loader type.");
        }


        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(LoaderType))
            {
                ModelState.AddModelError(nameof(LoaderType), "Select a loader type.");
                return Page();
            }

            if (UploadFile == null || UploadFile.Length == 0)
            {
                ModelState.AddModelError(nameof(UploadFile), "Select a file to upload.");
                return Page();
            }

            if (UploadFile.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(UploadFile), "The file exceeds the 10 MB limit.");
                return Page();
            }

            try
            {

                var loader = LoaderType.Trim();

                if (string.Equals(loader, "Item", StringComparison.OrdinalIgnoreCase))
                {
                    var (success, result, message) = await _apiClient.UploadDataAsync(UploadFile, cancellationToken);
                    if (!success || result == null)
                    {
                        TempData["UploadResult"] = string.IsNullOrWhiteSpace(message) ? "Upload failed." : message;
                        TempData["UploadResultType"] = "danger";
                        TempData.Remove("UploadErrors");
                        return RedirectToPage();
                    }

                    var summary = $"Processed {result.TotalRows} row(s). Created {result.ItemsCreated} item(s), updated {result.ItemsUpdated}, skipped {result.RowsSkipped}.";
                    var hasRowErrors = (result.Errors?.Count ?? 0) > 0;
                    TempData["UploadResult"] = summary;
                    TempData["UploadResultType"] = hasRowErrors ? "warning" : "success";

                    if (hasRowErrors)
                    {
                        TempData["UploadErrors"] = string.Join("||", result.Errors!);
                    }
                    else
                    {
                        TempData.Remove("UploadErrors");
                    }
                }
                else if (string.Equals(loader, "Stock", StringComparison.OrdinalIgnoreCase))
                {
                   var  (success, result, message) = await _apiClient.UploadStockAsync(UploadFile, cancellationToken);
                    if (!success || result == null)
                    {
                        TempData["UploadResult"] = string.IsNullOrWhiteSpace(message) ? "Upload failed." : message;
                        TempData["UploadResultType"] = "danger";
                        TempData.Remove("UploadErrors");
                        return RedirectToPage();
                    }

                    var summary = $"Processed {result.TotalRows} row(s). Updated stock entries. Skipped {result.RowsSkipped}.";
                    var hasRowErrors = (result.Errors?.Count ?? 0) > 0;
                    TempData["UploadResult"] = summary;
                    TempData["UploadResultType"] = hasRowErrors ? "warning" : "success";

                    if (hasRowErrors)
                    {
                        TempData["UploadErrors"] = string.Join("||", result.Errors!);
                    }
                    else
                    {
                        TempData.Remove("UploadErrors");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(LoaderType), "Invalid loader selected.");
                    return Page();
                }
                    
            }
            catch (ApiUnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload data file {FileName}", UploadFile.FileName);
                TempData["UploadResult"] = "Upload failed. Please try again.";
                TempData["UploadResultType"] = "danger";
                TempData.Remove("UploadErrors");
            }

            return RedirectToPage();
        }
    }
}
