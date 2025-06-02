using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    /// <summary>
    /// Workflow instance for tracking current state
    /// </summary>
    public class WorkflowInstance
    {
        public int InstanceId { get; set; }
        public int WorkflowId { get; set; }
        public int EntityId { get; set; }

        [Required, MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        public int CurrentStepId { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = WorkflowStatusConstants.Active;

        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? LastTransitionDate { get; set; }

        [Required, MaxLength(255)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CompletedBy { get; set; }

        public string? Data { get; set; } // JSON data for the instance
        public string? Variables { get; set; } // JSON variables for conditions
        public int Priority { get; set; } = WorkflowPriorityConstants.Normal;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? AssignedTo { get; set; } // Current assignee
        public DateTime? DueDate { get; set; }

        // Navigation properties
        public Workflow? Workflow { get; set; }
        public WorkflowStep? CurrentStep { get; set; }
        public ICollection<WorkflowHistory> History { get; set; } = new List<WorkflowHistory>();
    }
}
