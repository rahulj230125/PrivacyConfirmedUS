using Microsoft.AspNetCore.Http;
using PrivacyConfirmedModel;

namespace PrivacyConfirmedBAL.Interfaces
{
    #region Resource File Service Interface
    /// <summary>
    /// Interface for Resource File business logic operations
    /// </summary>
    public interface IResourceFileService
    {
        /// <summary>
        /// Processes and saves an uploaded file
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="uploadPath">Path where files should be saved</param>
        /// <returns>Result object indicating success or failure with message</returns>
        Task<ServiceResult> SaveFileAsync(IFormFile file, string uploadPath);

        /// <summary>
        /// Retrieves all active resource files
        /// </summary>
        /// <returns>List of all non-deleted resource files</returns>
        Task<List<ResourceFileModel>> GetAllFilesAsync();

        /// <summary>
        /// Retrieves a resource file by ID
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Resource file or null if not found</returns>
        Task<ResourceFileModel?> GetFileByIdAsync(int id);

        /// <summary>
        /// Deletes a resource file (soft delete)
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Result object indicating success or failure</returns>
        Task<ServiceResult> DeleteFileAsync(int id);

        /// <summary>
        /// Validates the uploaded file
        /// </summary>
        /// <param name="file">File to validate</param>
        /// <returns>Validation result with any error messages</returns>
        ServiceResult ValidateFile(IFormFile file);

        /// <summary>
        /// Gets the physical file path for a resource file
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>Physical file path or null if not found</returns>
        Task<string?> GetPhysicalFilePathAsync(int id);
    }
    #endregion
}
