using Microsoft.AspNetCore.Mvc;
using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedModel;

namespace PrivacyConfirmedAPI.Controllers
{
    /// <summary>
    /// API Controller for Resource Center operations
    /// Provides endpoints for managing resource files
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ResourceCenterController : ControllerBase
    {
        #region Fields

        private readonly IResourceFileService _resourceFileService;
        private readonly ILogger<ResourceCenterController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ResourceCenterController
        /// </summary>
        public ResourceCenterController(
            IResourceFileService resourceFileService,
            ILogger<ResourceCenterController> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _resourceFileService = resourceFileService ?? throw new ArgumentNullException(nameof(resourceFileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region API Endpoints

        /// <summary>
        /// Upload a new resource file
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <returns>Result indicating success or failure</returns>
        /// <response code="200">File uploaded successfully</response>
        /// <response code="400">Invalid file or validation error</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB limit
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                _logger.LogInformation("API: Received file upload request: {FileName}", file?.FileName);

                if (file == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "No file provided",
                        Errors = new List<string> { "File is required" }
                    });
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
                    _logger.LogInformation("API: File uploaded successfully: {FileName}", file.FileName);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = result.Message,
                        Data = new { FileName = file.FileName }
                    });
                }
                else
                {
                    _logger.LogWarning("API: File upload failed: {Message}", result.Message);

                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error uploading file");

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while uploading the file",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Get all resource files
        /// </summary>
        /// <returns>List of all resource files</returns>
        /// <response code="200">Returns the list of files</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ResourceFileModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                _logger.LogInformation("API: Retrieving all resource files");

                var files = await _resourceFileService.GetAllFilesAsync();

                return Ok(new ApiResponse<List<ResourceFileModel>>
                {
                    Success = true,
                    Message = $"Retrieved {files.Count} file(s)",
                    Data = files
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error retrieving resource files");

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving files",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Get a resource file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>File information</returns>
        /// <response code="200">Returns the file information</response>
        /// <response code="404">File not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ResourceFileModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFileById(int id)
        {
            try
            {
                _logger.LogInformation("API: Retrieving file with ID {Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid file ID",
                        Errors = new List<string> { "ID must be greater than 0" }
                    });
                }

                var file = await _resourceFileService.GetFileByIdAsync(id);

                if (file == null)
                {
                    _logger.LogWarning("API: File with ID {Id} not found", id);

                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"File with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<ResourceFileModel>
                {
                    Success = true,
                    Message = "File retrieved successfully",
                    Data = file
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error retrieving file with ID {Id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the file",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Download a file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>File stream</returns>
        /// <response code="200">Returns the file</response>
        /// <response code="404">File not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                _logger.LogInformation("API: Download requested for file ID: {Id}", id);

                var file = await _resourceFileService.GetFileByIdAsync(id);

                if (file == null)
                {
                    _logger.LogWarning("API: File with ID {Id} not found", id);
                    
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "File not found"
                    });
                }

                if (!System.IO.File.Exists(file.FilePath))
                {
                    _logger.LogWarning("API: Physical file not found at path: {FilePath}", file.FilePath);
                    
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "File not found on server"
                    });
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(file.FilePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                _logger.LogInformation("API: File downloaded successfully: {FileName}", file.FileName);

                return File(memory, GetContentType(file.FileExtension), file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error downloading file with ID: {Id}", id);
                
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while downloading the file"
                });
            }
        }

        /// <summary>
        /// Delete a file by ID (soft delete)
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Result indicating success or failure</returns>
        /// <response code="200">File deleted successfully</response>
        /// <response code="404">File not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                _logger.LogInformation("API: Delete requested for file ID: {Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid file ID",
                        Errors = new List<string> { "ID must be greater than 0" }
                    });
                }

                var result = await _resourceFileService.DeleteFileAsync(id);

                if (result.Success)
                {
                    _logger.LogInformation("API: File deleted successfully: ID {Id}", id);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = result.Message
                    });
                }
                else
                {
                    _logger.LogWarning("API: Failed to delete file: {Message}", result.Message);

                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error deleting file with ID: {Id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting the file",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Health check endpoint for Resource Center API
        /// </summary>
        /// <returns>API status</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "ResourceCenter",
                version = "1.0.0"
            });
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
    }
}
