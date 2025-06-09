using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Represents a claim assigned to a specific user
    /// </summary>
    public class UserClaim
    {
        public int UserClaimID { get; set; }
        public int UserID { get; set; }
        public int TenantID { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
