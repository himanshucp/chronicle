using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string SubscriptionLevel { get; set; }
        public int MaxUsers { get; set; }
        public int MaxProjects { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<TenantSetting> TenantSettings { get; set; }
        public virtual ICollection<TenantModuleAccess> TenantModuleAccess { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<HierarchyLevel> HierarchyLevels { get; set; }
        public virtual ICollection<CompanyRole> CompanyRoles { get; set; }
        public virtual ICollection<AccessModule> AccessModules { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<ProjectSection> ProjectSections { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
