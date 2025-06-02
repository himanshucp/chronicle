using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class HierarchyLevel
    {
        public int LevelID { get; set; }
        public string LevelName { get; set; }
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
