using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class Contract
    {
        public int ContractID { get; set; }
        public int TenantID { get; set; }
        public string ContractExternalID { get; set; }
        public string ContractTitle { get; set; }
        public int ProjectID { get; set; }

        public string Location { get; set; }
        public int? SectionID { get; set; }
        public int CompanyID { get; set; }
        public int CompanyRoleID { get; set; }
        public int? ParentContractID { get; set; }
        public int HierarchyLevelID { get; set; }
        public string ContractType { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? RetentionPercentage { get; set; }
        public decimal? RetentionAmount { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
       
        public DateTime? EndDate { get; set; }
        public DateTime? SignDate { get; set; }
        public string Status { get; set; }
        public int? ContractManagerID { get; set; }
        public string PaymentTerms { get; set; }
        public bool? InsuranceRequired { get; set; }
        public bool? InsuranceVerified { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Notes { get; set; }
        public bool? IsActive { get; set; }

        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual ProjectSection Section { get; set; }
        public virtual Company Company { get; set; }
        public virtual CompanyRole CompanyRole { get; set; }
        public virtual Contract ParentContract { get; set; }
        public virtual HierarchyLevel HierarchyLevel { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Contract> ChildContracts { get; set; }
        public virtual ICollection<ContractEmployee> ContractEmployees { get; set; }
    }
}
