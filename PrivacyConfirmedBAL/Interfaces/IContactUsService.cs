using PrivacyConfirmedModel;

namespace PrivacyConfirmedBAL.Interfaces
{
    #region Contact Us Service Interface
    /// <summary>
    /// Interface for Contact Us business logic operations
    /// </summary>
    public interface IContactUsService
    {
        /// <summary>
        /// Processes and saves a new contact request
        /// </summary>
        /// <param name="model">Contact Us model with user information</param>
        /// <returns>Result object indicating success or failure with message</returns>
        Task<ServiceResult> SaveContactAsync(ContactUsModel model);

        /// <summary>
        /// Retrieves all contact records
        /// </summary>
        /// <returns>List of all contact records</returns>
        Task<List<ContactUsModel>> GetAllContactsAsync();

        /// <summary>
        /// Retrieves a contact record by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact record or null if not found</returns>
        Task<ContactUsModel?> GetContactByIdAsync(int id);

        /// <summary>
        /// Validates the contact model
        /// </summary>
        /// <param name="model">Contact Us model to validate</param>
        /// <returns>Validation result with any error messages</returns>
        ServiceResult ValidateContact(ContactUsModel model);
    }

    /// <summary>
    /// Service result class for returning operation results
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
    #endregion
}
