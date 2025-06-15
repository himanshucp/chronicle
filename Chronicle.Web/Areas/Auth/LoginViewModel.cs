using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Auth
{
    /// <summary>
    /// Login view model
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Return URL after successful login
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Indicates if the form has been submitted
        /// </summary>
        public bool IsSubmitted { get; set; }

        /// <summary>
        /// Error message to display
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Success message to display
        /// </summary>
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Indicates if account is locked
        /// </summary>
        public bool IsAccountLocked { get; set; }

        /// <summary>
        /// Lockout end time if account is locked
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int FailedAttempts { get; set; }

        /// <summary>
        /// Maximum allowed failed attempts before lockout
        /// </summary>
        public int MaxFailedAttempts { get; set; } = 5;

        /// <summary>
        /// Available external login providers
        /// </summary>
        //public List<ExternalLoginProvider> ExternalProviders { get; set; } = new List<ExternalLoginProvider>();

        /// <summary>
        /// Indicates if external login is enabled
        /// </summary>
        //public bool ExternalLoginEnabled => ExternalProviders.Any();

        /// <summary>
        /// Client IP address for logging purposes
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// User agent for logging purposes
        /// </summary>
        public string UserAgent { get; set; }
    }
}
