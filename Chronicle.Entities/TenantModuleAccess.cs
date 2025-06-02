using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class TenantModuleAccess
    {
        public int TenantModuleID { get; set; }
        public int TenantID { get; set; }
        public int ModuleID { get; set; }
        public bool HasAccess { get; set; }
        public string AccessLevel { get; set; }
        public int MaxLicensedUsers { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Tenant Tenant { get; set; }
        public virtual AccessModule AccessModule { get; set; }
    }
}
