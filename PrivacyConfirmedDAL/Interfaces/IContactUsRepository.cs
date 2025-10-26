using PrivacyConfirmedModel;

namespace PrivacyConfirmedDAL.Interfaces
{
    #region Contact Us Repository Interface
    /// <summary>
    /// Interface for Contact Us data access operations
    /// </summary>
    public interface IContactUsRepository
    {
        /// <summary>
        /// Inserts a new contact record into the database
        /// </summary>
        /// <param name="model">Contact Us model with user information</param>
        /// <returns>True if insert was successful, false otherwise</returns>
        Task<bool> InsertContactAsync(ContactUsModel model);

        /// <summary>
        /// Gets all contact records from the database
        /// </summary>
        /// <returns>List of all contact records</returns>
        Task<List<ContactUsModel>> GetAllContactsAsync();

        /// <summary>
        /// Gets a contact record by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact record or null if not found</returns>
        Task<ContactUsModel?> GetContactByIdAsync(int id);
    }
    #endregion
}
