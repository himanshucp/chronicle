using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Project
    {
        public int ProjectID { get; set; }
        public int TenantID { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Description { get; set; }
        public string? ProjectType { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public int OwnerCompanyID { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? ProjectStatus { get; set; }
        public int? ProjectManagerID { get; set; }
        public string? PermitNumber { get; set; }
        public DateTime? PermitIssueDate { get; set; }
        public DateTime? PermitExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Company OwnerCompany { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<ProjectSection> Sections { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
