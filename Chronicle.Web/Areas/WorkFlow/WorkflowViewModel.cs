using Chronicle.Web.Areas.Contract;
using Chronicle.Web.Code;
using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web.Areas.WorkFlow
{
    public class WorkflowViewModel
    {
        public int WorkflowId { get; set; }

        [Required, MaxLength(255)]
        public string WorkflowName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Module { get; set; } = string.Empty; // e.g., "InspectionRequest", "PurchaseOrder"

        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        [Required, MaxLength(255)]
        public string CreatedBy { get; set; } = string.Empty;

        public string? Configuration { get; set; } // JSON configuration
        public int Version { get; set; } = 1;
        public DateTime? LastModified { get; set; }

        [MaxLength(255)]
        public string? LastModifiedBy { get; set; }
    }


}
