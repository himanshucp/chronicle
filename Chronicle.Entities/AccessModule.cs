using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class AccessModule
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }

        // Navigation properties
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<TenantModuleAccess> TenantModuleAccess { get; set; }
        public virtual ICollection<EmployeeModuleAccess> EmployeeModuleAccess { get; set; }
    }
}
