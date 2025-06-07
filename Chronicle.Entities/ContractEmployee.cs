using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class ContractEmployee
    {
        public int ContractEmployeeID { get; set; }
        public int TenantID { get; set; }
        public int ContractID { get; set; }
        public int EmployeeID { get; set; }
        public int RoleID { get; set; }
        public int? LineManagerID { get; set; }
        public decimal? HourlyRate { get; set; }
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }
        public DateTime? DateActivated { get; set; }
        public DateTime? DateDeactivated { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }

        // Navigation properties
        public virtual Contract Contract { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Role Role { get; set; }
        public virtual Employee LineManager { get; set; }
        public virtual ICollection<EmployeeModuleAccess> ModuleAccess { get; set; }
    }
}
