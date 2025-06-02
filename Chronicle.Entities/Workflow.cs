using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Base workflow entity
    /// </summary>
    public class Workflow
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
