using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class ContractEmployeeRole
    {
        public int ContractRoleID { get; set; }
        public int TenantID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
