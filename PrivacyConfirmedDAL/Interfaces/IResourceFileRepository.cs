using PrivacyConfirmedModel;

namespace PrivacyConfirmedDAL.Interfaces
{
    #region Resource File Repository Interface
    /// <summary>
    /// Interface for Resource File data access operations
    /// </summary>
    public interface IResourceFileRepository
    {
        /// <summary>
        /// Inserts a new resource file record into the database
        /// </summary>
        /// <param name="model">Resource file model with file information</param>
        /// <returns>True if insert was successful, false otherwise</returns>
        Task<bool> InsertResourceFileAsync(ResourceFileModel model);

        /// <summary>
        /// Retrieves all non-deleted resource files
        /// </summary>
        /// <returns>List of all active resource files</returns>
        Task<List<ResourceFileModel>> GetAllResourceFilesAsync();

        /// <summary>
        /// Retrieves a resource file record by ID
        /// </summary>
        /// <param name="id">Resource file ID</param>
        /// <returns>Resource file record or null if not found</returns>
        Task<ResourceFileModel?> GetResourceFileByIdAsync(int id);

        /// <summary>
        /// Soft deletes a resource file by setting IsDeleted = true
        /// </summary>
        /// <param name="id">Resource file ID</param>
        /// <returns>True if delete was successful, false otherwise</returns>
        Task<bool> DeleteResourceFileAsync(int id);
    }
    #endregion
}
