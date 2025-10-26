using System.ComponentModel.DataAnnotations;

namespace PrivacyConfirmedModel
{
    #region Contact Us Model
    /// <summary>
    /// Model class for Contact Us form
    /// Contains validation attributes for user input
    /// </summary>
    public class ContactUsModel
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the contact record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the person contacting
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Company name
        /// </summary>
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 150 characters")]
        [Display(Name = "Company Name")]
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Mobile number of the contact
        /// </summary>
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit mobile number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the contact
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the contact was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #endregion
    }
    #endregion
}
