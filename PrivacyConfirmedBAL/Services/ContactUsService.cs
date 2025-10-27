using PrivacyConfirmedModel;
using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedBAL.Interfaces;
using System.Text.RegularExpressions;

namespace PrivacyConfirmedBAL.Services
{
    #region Contact Us Service
    /// <summary>
    /// Service class for Contact Us business logic
    /// Handles validation, data transformation, and orchestrates DAL operations
    /// </summary>
    public class ContactUsService : IContactUsService
    {
        #region Private Fields
        private readonly IContactUsRepository _repository;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ContactUsService
        /// </summary>
        /// <param name="repository">Contact Us repository for data access</param>
        public ContactUsService(IContactUsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Processes and saves a new contact request
        /// Performs business validation and data transformation before saving
        /// </summary>
        /// <param name="model">Contact Us model with user information</param>
        /// <returns>Result object indicating success or failure with message</returns>
        public async Task<ServiceResult> SaveContactAsync(ContactUsModel model)
        {
            var result = new ServiceResult();

            try
            {
                // Validate the model
                var validationResult = ValidateContact(model);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Business logic: Transform data if needed
                model.Name = SanitizeInput(model.Name);
                model.Company = SanitizeInput(model.Company);
                model.Email = model.Email.Trim().ToLower();
                model.MobileNumber = model.MobileNumber.Trim();
                model.CreatedAt = DateTime.UtcNow;

                // Check for duplicate email (business rule)
                var existingContacts = await _repository.GetAllContactsAsync();
                if (existingContacts.Any(c => c.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Success = false;
                    result.Message = "A contact with this email address already exists.";
                    result.Errors.Add("Duplicate email address");
                    return result;
                }

                // Save to database
                bool success = await _repository.InsertContactAsync(model);

                if (success)
                {
                    result.Success = true;
                    result.Message = "Thank you for contacting us! We will get back to you shortly.";
                }
                else
                {
                    result.Success = false;
                    result.Message = "Failed to save contact information. Please try again.";
                    result.Errors.Add("Database insert failed");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "An error occurred while processing your request. Please try again later.";
                result.Errors.Add($"Exception: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Retrieves all contact records
        /// </summary>
        /// <returns>List of all contact records</returns>
        public async Task<List<ContactUsModel>> GetAllContactsAsync()
        {
            try
            {
                return await _repository.GetAllContactsAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving contacts: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a contact record by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact record or null if not found</returns>
        public async Task<ContactUsModel?> GetContactByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid contact ID", nameof(id));
                }

                return await _repository.GetContactByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving contact by ID: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validates the contact model using business rules
        /// </summary>
        /// <param name="model">Contact Us model to validate</param>
        /// <returns>Validation result with any error messages</returns>
        public ServiceResult ValidateContact(ContactUsModel model)
        {
            var result = new ServiceResult { Success = true };

            if (model == null)
            {
                result.Success = false;
                result.Message = "Contact information cannot be null";
                result.Errors.Add("Model is null");
                return result;
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                result.Success = false;
                result.Errors.Add("Name is required");
            }
            else if (model.Name.Length < 2 || model.Name.Length > 100)
            {
                result.Success = false;
                result.Errors.Add("Name must be between 2 and 100 characters");
            }

            // Validate Company
            if (string.IsNullOrWhiteSpace(model.Company))
            {
                result.Success = false;
                result.Errors.Add("Company name is required");
            }
            else if (model.Company.Length < 2 || model.Company.Length > 150)
            {
                result.Success = false;
                result.Errors.Add("Company name must be between 2 and 150 characters");
            }

            // Validate Mobile Number
            if (string.IsNullOrWhiteSpace(model.MobileNumber))
            {
                result.Success = false;
                result.Errors.Add("Mobile number is required");
            }
            else if (!Regex.IsMatch(model.MobileNumber, @"^[0-9]{10}$"))
            {
                result.Success = false;
                result.Errors.Add("Mobile number must be a valid 10-digit number");
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                result.Success = false;
                result.Errors.Add("Email is required");
            }
            else if (!IsValidEmail(model.Email))
            {
                result.Success = false;
                result.Errors.Add("Email address is not valid");
            }

            if (!result.Success)
            {
                result.Message = "Validation failed. Please check the errors.";
            }

            return result;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Sanitizes input by trimming whitespace and removing potentially harmful characters
        /// </summary>
        /// <param name="input">Input string to sanitize</param>
        /// <returns>Sanitized string</returns>
        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim and remove excessive whitespace
            input = input.Trim();
            input = Regex.Replace(input, @"\s+", " ");

            return input;
        }

        /// <summary>
        /// Validates email format using regex
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if email is valid, false otherwise</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use a comprehensive email validation regex
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
    #endregion
}
