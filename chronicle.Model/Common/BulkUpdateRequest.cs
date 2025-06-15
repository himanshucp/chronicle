using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chronicle.Model.Common
{
    public class BulkUpdateRequest
    {
        public List<int> CompanyIds { get; set; } = new List<int>();
        public string Action { get; set; } = "";
        public string NewValue { get; set; } = "";
        public int TenantId { get; set; } = 1;
    }
}
