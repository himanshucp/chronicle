using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chronicle.Model.Common
{
    public class ValidateUniqueRequest
    {
        public string Field { get; set; } = "";
        public string Value { get; set; } = "";
        public int? ExcludeId { get; set; }
        public int TenantId { get; set; } = 1;
    }
}
