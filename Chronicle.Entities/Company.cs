using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Company
    {
        public int CompanyID { get; set; }

        public int TenantID { get; set; }

        public string Name { get; set; }

        public string CompanyType { get; set; }

        public string Location { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string ContactPerson { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string TaxNumber { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime? LicenseExpiryDate { get; set; }

        public string InsuranceDetails { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string WebSite { get; set; }

        public long TotalContract { get; set; }

        public long TotalUsers { get; set; }

        public long TotalEmployee { get; set; }

        public bool? IsActive { get; set; }

        // Navigation properties
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Project> OwnedProjects { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
