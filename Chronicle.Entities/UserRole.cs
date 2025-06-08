using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between users and roles
    /// </summary>
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int TenantID { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
