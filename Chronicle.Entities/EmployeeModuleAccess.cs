using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class EmployeeModuleAccess
    {
        public int AccessID { get; set; }
        public int ContractEmployeeID { get; set; }
        public int ModuleID { get; set; }
        public bool HasAccess { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ContractEmployee ContractEmployee { get; set; }
        public virtual AccessModule AccessModule { get; set; }
    }
}
