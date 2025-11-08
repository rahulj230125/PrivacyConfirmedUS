using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PrivacyConfirmed.Models;
using PrivacyConfirmedModel;
using PrivacyConfirmedBAL.Interfaces;

namespace PrivacyConfirmed.Controllers
{
    public class ResourceCenterController : Controller
    {
        private readonly ILogger<ResourceCenterController> _logger;
        private readonly IResourceFileService _resourceFileService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ResourceCenterController(
            ILogger<ResourceCenterController> logger,
            IResourceFileService resourceFileService,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _logger = logger;
            _resourceFileService = resourceFileService;
            _environment = environment;
            _configuration = configuration;
        }

        #region Resource Center Actions

        /// <summary>
        /// GET: Display the Resource Center page with file upload form and grid
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var files = await _resourceFileService.GetAllFilesAsync();

                var viewModel = new FileUploadViewModel
                {
                    ResourceFiles = files ?? new List<ResourceFileModel>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Resource Center");
                TempData["ErrorMessage"] = "An error occurred while loading resources.";
                return View(new FileUploadViewModel());
            }
        }

        /// <summary>
        /// POST: Handle file upload
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    TempData["ErrorMessage"] = "Please select a file to upload.";
                    return RedirectToAction(nameof(Index));
                }

                // Get upload path from configuration
                var uploadPath = _configuration["FileUploadSettings:UploadPath"]
                    ?? Path.Combine(_environment.ContentRootPath, "UploadedFiles");

                // Ensure the path is absolute
                if (!Path.IsPathRooted(uploadPath))
                {
                    uploadPath = Path.Combine(_environment.ContentRootPath, uploadPath);
                }

                var result = await _resourceFileService.SaveFileAsync(file, uploadPath);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    if (result.Errors?.Any() == true)
                    {
                        TempData["ErrorDetails"] = string.Join(", ", result.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                TempData["ErrorMessage"] = "An unexpected error occurred while uploading the file.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: Download file by ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                _logger.LogInformation("Download requested for file ID: {Id}", id);

                var file = await _resourceFileService.GetFileByIdAsync(id);

                if (file == null)
                {
                    _logger.LogWarning("File with ID {Id} not found", id);
                    TempData["ErrorMessage"] = "File not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (!System.IO.File.Exists(file.FilePath))
                {
                    _logger.LogWarning("Physical file not found at path: {FilePath}", file.FilePath);
                    TempData["ErrorMessage"] = "File not found on server.";
                    return RedirectToAction(nameof(Index));
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(file.FilePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                _logger.LogInformation("File downloaded successfully: {FileName}", file.FileName);

                var contentType = GetContentType(file.FileExtension);
                return File(memory, contentType, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file with ID: {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while downloading the file.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Delete file by ID (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Delete requested for file ID: {Id}", id);

                var result = await _resourceFileService.DeleteFileAsync(id);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file with ID: {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the file.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the MIME content type based on file extension
        /// </summary>
        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".zip" => "application/zip",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
