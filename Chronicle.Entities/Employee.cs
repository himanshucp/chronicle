using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int TenantID { get; set; }
        public int CompanyID { get; set; }
        public int? DepartmentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }
        public string Position { get; set; }
        public string EmployeeType { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public bool? HasConstructionLicense { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public bool? SafetyTrainingCompleted { get; set; }
        public DateTime? SafetyTrainingDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<ContractEmployee> ContractAssignments { get; set; }
        public virtual ICollection<ContractEmployee> ReportsToMe { get; set; }
    }
}
