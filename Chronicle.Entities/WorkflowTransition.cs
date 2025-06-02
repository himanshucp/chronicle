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
    /// Workflow transition definition
    /// </summary>
    public class WorkflowTransition
    {
        public int TransitionId { get; set; }
        public int WorkflowId { get; set; }
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }

        [Required, MaxLength(100)]
        public string ActionCode { get; set; } = string.Empty; // e.g., "SUBMIT", "APPROVE", "REJECT"

        [Required, MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        public string? Condition { get; set; } // JSON conditions for transition
        public string? AllowedRoles { get; set; } // JSON array of roles
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; } = 0; // Higher number = higher priority
        public string? Configuration { get; set; } // JSON configuration

        public bool RequiresApproval { get; set; }
        public bool RequiresComments { get; set; }
        public string? NotificationRoles { get; set; } // JSON array of roles to notify

        // Navigation properties
        public Workflow? Workflow { get; set; }
        public WorkflowStep? FromStep { get; set; }
        public WorkflowStep? ToStep { get; set; }
    }

}
