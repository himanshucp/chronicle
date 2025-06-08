using Chronicle.Web.Code;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Companies
{
    public class CompanyModel  
    {
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Abbrivation is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Name Abbrivation be between 3 characters")]
        public string Abbrivation { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Location must be between 2 and 200 characters")]
        public string? Location { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Contact Person is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Contact Person must be between 2 and 200 characters")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Special characters are not allowed.")]
        public string? ContactPerson { get; set; }

        [RegularExpression(@"^\+[1-9]\d{1,14}$",
        ErrorMessage = "Enter a valid international phone number (e.g., +1234567890)")]
        public string? Phone { get; set; }

        [RegularExpression(@"^\+[1-9]\d{1,14}$",
        ErrorMessage = "Enter a valid international phone number (e.g., +1234567890)")]
        public string? Fax { get; set; }

        public string? TaxNumber { get; set; }

        public string? LicenseNumber { get; set; }

        [Display(Name = "License Expiry Date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LicenseExpiryDate { get; set; }

        public string? InsuranceDetails { get; set; }

        [RegularExpression(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/\S*)?$", ErrorMessage = "Enter a valid website URL.")]
        public string? WebSite { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; } = 1;

    }
}
