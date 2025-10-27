using Microsoft.AspNetCore.Mvc;
using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedModel;

namespace PrivacyConfirmedAPI.Controllers
{
    /// <summary>
    /// API Controller for Contact Us operations
    /// Provides endpoints for managing contact form submissions
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContactUsController : ControllerBase
    {
        #region Fields

        private readonly IContactUsService _contactUsService;
        private readonly ILogger<ContactUsController> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContactUsController
        /// </summary>
        /// <param name="contactUsService">Contact Us service for business logic</param>
        /// <param name="logger">Logger for tracking operations</param>
        public ContactUsController(
            IContactUsService contactUsService,
            ILogger<ContactUsController> logger)
        {
            _contactUsService = contactUsService ?? throw new ArgumentNullException(nameof(contactUsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region API Endpoints

        /// <summary>
        /// Submit a new contact form
        /// </summary>
        /// <param name="model">Contact information</param>
        /// <returns>Result indicating success or failure</returns>
        /// <response code="200">Contact saved successfully</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsModel model)
        {
            try
            {
                _logger.LogInformation("Received contact form submission from {Email}", model?.Email);

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors));

                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                if (model == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Contact information is required",
                        Errors = new List<string> { "Request body cannot be null" }
                    });
                }

                // Save contact using service
                var result = await _contactUsService.SaveContactAsync(model);

                if (result.Success)
                {
                    _logger.LogInformation("Contact saved successfully for {Email}", model.Email);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = result.Message,
                        Data = new { ContactEmail = model.Email }
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to save contact for {Email}: {Message}", model.Email, result.Message);

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
                _logger.LogError(ex, "Error processing contact form submission");

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while processing your request",
                    Errors = new List<string> { "Internal server error. Please try again later." }
                });
            }
        }

        /// <summary>
        /// Get all contact submissions
        /// </summary>
        /// <returns>List of all contacts</returns>
        /// <response code="200">Returns the list of contacts</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ContactUsModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                _logger.LogInformation("Retrieving all contacts");

                var contacts = await _contactUsService.GetAllContactsAsync();

                return Ok(new ApiResponse<List<ContactUsModel>>
                {
                    Success = true,
                    Message = $"Retrieved {contacts.Count} contact(s)",
                    Data = contacts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts");

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving contacts",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Get a contact by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact information</returns>
        /// <response code="200">Returns the contact</response>
        /// <response code="404">Contact not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ContactUsModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContactById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving contact with ID {Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid contact ID",
                        Errors = new List<string> { "ID must be greater than 0" }
                    });
                }

                var contact = await _contactUsService.GetContactByIdAsync(id);

                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {Id} not found", id);

                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<ContactUsModel>
                {
                    Success = true,
                    Message = "Contact retrieved successfully",
                    Data = contact
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact with ID {Id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the contact",
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        /// <summary>
        /// Health check endpoint
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
                version = "1.0.0"
            });
        }

        #endregion
    }

    #region Response Models

    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// List of error messages if operation failed
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Additional data returned by the operation
        /// </summary>
        public object? Data { get; set; }
    }

    /// <summary>
    /// Generic API response wrapper with typed data
    /// </summary>
    /// <typeparam name="T">Type of data returned</typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Typed data returned by the operation
        /// </summary>
        public new T? Data { get; set; }
    }

    #endregion
}
