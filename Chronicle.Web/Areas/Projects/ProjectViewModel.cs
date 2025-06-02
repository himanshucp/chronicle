using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.Projects
{
    public record ProjectViewModel
    {

        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters")]
        public string ProjectName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int OwnerCompanyID { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? ActualStartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public int TenantID { get; set; }

        public string CompanyName { get; set; }

        // For dropdown list
        [BindNever]
        public SelectList CompanySelectList { get; set; }
    }
}
