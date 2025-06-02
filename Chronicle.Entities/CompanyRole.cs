using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class CompanyRole
    {
        public int CompanyRoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }

        // Navigation properties
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
