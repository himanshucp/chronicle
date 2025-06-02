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
    /// Workflow execution history
    /// </summary>
    public class WorkflowHistory
    {
        public int HistoryId { get; set; }
        public int InstanceId { get; set; }
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }

        [Required, MaxLength(100)]
        public string ActionCode { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Action { get; set; } = string.Empty;

        public string? Comments { get; set; }
        public DateTime TransitionDate { get; set; }

        [Required, MaxLength(255)]
        public string ActionBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ActionByRole { get; set; }

        public string? AdditionalData { get; set; } // JSON data
        public double? DurationMinutes { get; set; }

        [MaxLength(255)]
        public string? AssignedTo { get; set; } // Who it was assigned to after transition

        public DateTime? DueDate { get; set; } // Due date after transition

        // Navigation properties
        public WorkflowInstance? Instance { get; set; }
        public WorkflowStep? FromStep { get; set; }
        public WorkflowStep? ToStep { get; set; }
    }

}
