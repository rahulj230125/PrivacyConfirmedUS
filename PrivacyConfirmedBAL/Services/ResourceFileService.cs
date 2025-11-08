using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedModel;

namespace PrivacyConfirmedBAL.Services
{
    #region Resource File Service
    /// <summary>
    /// Service class for Resource File business logic
    /// Handles file upload, validation, and management operations
    /// </summary>
    public class ResourceFileService : IResourceFileService
    {
        #region Private Fields
        private readonly IResourceFileRepository _repository;
        private readonly ILogger<ResourceFileService> _logger;

        // Allowed file extensions
        private static readonly string[] AllowedExtensions = { ".zip", ".doc", ".docx", ".xlsx", ".xls" };
        
        // Maximum file size (10 MB)
        private const long MaxFileSize = 10 * 1024 * 1024;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ResourceFileService
        /// </summary>
        /// <param name="repository">Resource file repository</param>
        /// <param name="logger">Logger instance</param>
        public ResourceFileService(
            IResourceFileRepository repository,
            ILogger<ResourceFileService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Saves an uploaded file to the server and database
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="uploadPath">Path where files should be saved</param>
        /// <returns>Result indicating success or failure</returns>
        public async Task<ServiceResult> SaveFileAsync(IFormFile file, string uploadPath)
        {
            try
            {
                _logger.LogInformation("Starting file upload process for file: {FileName}", file?.FileName);

                // Validate the file
                var validationResult = ValidateFile(file);
                if (!validationResult.Success)
                {
                    _logger.LogWarning("File validation failed: {Message}", validationResult.Message);
                    return validationResult;
                }

                // Ensure upload directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                    _logger.LogInformation("Created upload directory: {Path}", uploadPath);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(file!.FileName).ToLowerInvariant();
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{originalFileName}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved to disk: {FilePath}", filePath);

                // Create model and save to database
                var model = new ResourceFileModel
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    FileExtension = fileExtension,
                    CreatedDate = DateTime.UtcNow
                };

                var insertSuccess = await _repository.InsertResourceFileAsync(model);

                if (insertSuccess)
                {
                    _logger.LogInformation("File metadata saved to database successfully");
                    return new ServiceResult
                    {
                        Success = true,
                        Message = "File uploaded successfully!"
                    };
                }
                else
                {
                    // If database insert fails, delete the physical file
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogWarning("Deleted physical file due to database insert failure");
                    }

                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Failed to save file metadata to database",
                        Errors = new List<string> { "Database operation failed" }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", file?.FileName);
                return new ServiceResult
                {
                    Success = false,
                    Message = "An error occurred while uploading the file",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        /// <summary>
        /// Retrieves all active resource files
        /// </summary>
        /// <returns>List of all non-deleted resource files</returns>
        public async Task<List<ResourceFileModel>> GetAllFilesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all resource files");
                var files = await _repository.GetAllResourceFilesAsync();
                _logger.LogInformation("Retrieved {Count} resource files", files.Count);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource files");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a resource file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Resource file or null if not found</returns>
        public async Task<ResourceFileModel?> GetFileByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving resource file with ID: {Id}", id);
                var file = await _repository.GetResourceFileByIdAsync(id);
                
                if (file == null)
                {
                    _logger.LogWarning("Resource file with ID {Id} not found", id);
                }
                
                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource file with ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Deletes a resource file (soft delete)
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Result indicating success or failure</returns>
        public async Task<ServiceResult> DeleteFileAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete resource file with ID: {Id}", id);

                // Check if file exists
                var file = await _repository.GetResourceFileByIdAsync(id);
                if (file == null)
                {
                    _logger.LogWarning("Resource file with ID {Id} not found", id);
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "File not found",
                        Errors = new List<string> { $"No file found with ID {id}" }
                    };
                }

                // Soft delete in database
                var deleteSuccess = await _repository.DeleteResourceFileAsync(id);

                if (deleteSuccess)
                {
                    _logger.LogInformation("Resource file with ID {Id} deleted successfully", id);
                    
                    // Optionally delete physical file
                    // Uncomment the following lines if you want to delete the physical file
                    /*
                    if (File.Exists(file.FilePath))
                    {
                        File.Delete(file.FilePath);
                        _logger.LogInformation("Physical file deleted: {FilePath}", file.FilePath);
                    }
                    */

                    return new ServiceResult
                    {
                        Success = true,
                        Message = "File deleted successfully"
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Failed to delete file",
                        Errors = new List<string> { "Database operation failed" }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource file with ID: {Id}", id);
                return new ServiceResult
                {
                    Success = false,
                    Message = "An error occurred while deleting the file",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        /// <summary>
        /// Validates the uploaded file
        /// </summary>
        /// <param name="file">File to validate</param>
        /// <returns>Validation result with any error messages</returns>
        public ServiceResult ValidateFile(IFormFile? file)
        {
            var errors = new List<string>();

            // Check if file is null
            if (file == null)
            {
                errors.Add("No file selected");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Please select a file to upload",
                    Errors = errors
                };
            }

            // Check if file is empty
            if (file.Length == 0)
            {
                errors.Add("File is empty");
            }

            // Check file size
            if (file.Length > MaxFileSize)
            {
                errors.Add($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB");
            }

            // Check file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !AllowedExtensions.Contains(fileExtension))
            {
                errors.Add($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            // Check filename
            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                errors.Add("File name is invalid");
            }

            if (errors.Any())
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "File validation failed",
                    Errors = errors
                };
            }

            return new ServiceResult
            {
                Success = true,
                Message = "File is valid"
            };
        }

        /// <summary>
        /// Gets the physical file path for a resource file
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Physical file path or null if not found</returns>
        public async Task<string?> GetPhysicalFilePathAsync(int id)
        {
            try
            {
                var file = await _repository.GetResourceFileByIdAsync(id);
                return file?.FilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting physical file path for ID: {Id}", id);
                return null;
            }
        }

        #endregion
    }
    #endregion
}
