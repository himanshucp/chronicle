using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Employees
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }
        public int TenantID { get; set; } = 1;

        [Required(ErrorMessage = "Company is required")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Dicpline is required")]
        public int? DepartmentID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(200, ErrorMessage = "FirstName name cannot be longer than 200 characters")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "LastName Name is required")]
        [StringLength(200, ErrorMessage = "LastName name cannot be longer than 200 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [RegularExpression(@"^\+[1-9]\d{1,14}$",
        ErrorMessage = "Enter a valid international phone number (e.g., +1234567890)")]
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(200, ErrorMessage = "Position cannot be longer than 200 characters")]
        public string Position { get; set; }
        //public string EmployeeType { get; set; }
        //public string EmergencyContactName { get; set; }
        //public string EmergencyContactPhone { get; set; }
        //public bool? HasConstructionLicense { get; set; }
        //public string LicenseNumber { get; set; }
        //public DateTime? LicenseExpiryDate { get; set; }
        public bool? SafetyTrainingCompleted { get; set; }
        public DateTime? SafetyTrainingDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
