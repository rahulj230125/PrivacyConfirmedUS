using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PrivacyConfirmedModel
{
    #region Resource File Model
    /// <summary>
    /// Model class for uploaded resource files
    /// Contains file metadata and validation attributes
    /// </summary>
    public class ResourceFileModel
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the resource file record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the uploaded file
        /// </summary>
        [Required(ErrorMessage = "File name is required")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Full path where the file is stored on the server
        /// </summary>
        [Required(ErrorMessage = "File path is required")]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        [Display(Name = "File Path")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [Display(Name = "File Size (KB)")]
        public long FileSize { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        [StringLength(50, ErrorMessage = "File extension cannot exceed 50 characters")]
        [Display(Name = "File Type")]
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the file was uploaded
        /// </summary>
        [Display(Name = "Upload Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates if the file has been soft-deleted
        /// </summary>
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets file size formatted in appropriate unit (KB, MB, GB)
        /// </summary>
        public string GetFormattedFileSize()
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            else if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024.0:F2} KB";
            else if (FileSize < 1024 * 1024 * 1024)
                return $"{FileSize / (1024.0 * 1024.0):F2} MB";
            else
                return $"{FileSize / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }

        #endregion
    }

    /// <summary>
    /// View model for file upload operations
    /// </summary>
    public class FileUploadViewModel
    {
        /// <summary>
        /// File to be uploaded
        /// </summary>
        [Required(ErrorMessage = "Please select a file to upload")]
        [Display(Name = "Select File")]
        public IFormFile? File { get; set; }

        /// <summary>
        /// List of all resource files
        /// </summary>
        public List<ResourceFileModel> ResourceFiles { get; set; } = new List<ResourceFileModel>();

        /// <summary>
        /// Allowed file extensions
        /// </summary>
        public static readonly string[] AllowedExtensions = { ".zip", ".doc", ".docx", ".xlsx", ".xls" };

        /// <summary>
        /// Maximum file size in bytes (10 MB)
        /// </summary>
        public const long MaxFileSize = 10 * 1024 * 1024;
    }
    #endregion
}
