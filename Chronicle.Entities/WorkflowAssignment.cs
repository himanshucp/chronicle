using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{

    /// <summary>
    /// Workflow assignments for tracking current and historical assignments
    /// </summary>
    public class WorkflowAssignment
    {
        public int AssignmentId { get; set; }
        public int InstanceId { get; set; }
        public int StepId { get; set; }

        [Required, MaxLength(255)]
        public string AssignedTo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string AssignedRole { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [Required, MaxLength(255)]
        public string AssignedBy { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }


}
