using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class TenantSetting
    {
        public int SettingID { get; set; }
        public int TenantID { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string SettingDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // Navigation property
        public virtual Tenant Tenant { get; set; }
    }

}
