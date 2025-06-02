using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Contract
{
    public class ContractViewModel
    {
        public int ContractID { get; set; }
        public int TenantID { get; set; }

        [Required(ErrorMessage = "Contract Id is required")]
        public string ContractExternalID { get; set; }

        [Required(ErrorMessage = "Contract Title is required")]
        [StringLength(200, ErrorMessage = "Contract Title cannot be longer than 200 characters")]
        public string ContractTitle { get; set; }
        public int ProjectID { get; set; }
        public int? SectionID { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Company Role is required")]
        public int CompanyRoleID { get; set; }
        public int? ParentContractID { get; set; }

        [Required(ErrorMessage = "Contract Hierarchy level is required")]
       
        public int HierarchyLevelID { get; set; }

        public string Location { get; set; }
        public string ContractType { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? RetentionPercentage { get; set; }
        public decimal? RetentionAmount { get; set; }
        public DateTime? StartDate { get; set; }

        [DateRange("StartDate", ErrorMessage = "End date must be greater than start date")]
        public DateTime? EndDate { get; set; }
        public DateTime? SignDate { get; set; }
        public string? Status { get; set; }
        public int? ContractManagerID { get; set; }
        public string? PaymentTerms { get; set; }
        public bool? InsuranceRequired { get; set; }
        public bool? InsuranceVerified { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Notes { get; set; }
        public bool? IsActive { get; set; }
    }
}
