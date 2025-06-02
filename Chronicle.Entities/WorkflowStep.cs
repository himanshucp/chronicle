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
    /// Workflow step definition - flexible for any module
    /// </summary>
    public class WorkflowStep
    {
        public int StepId { get; set; }
        public int WorkflowId { get; set; }

        [Required, MaxLength(100)]
        public string StepCode { get; set; } = string.Empty; // e.g., "DRAFT", "APPROVAL", "ASSIGNMENT"

        [Required, MaxLength(255)]
        public string StepName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string StepType { get; set; } = string.Empty; // UserInput, Approval, Process, Final, etc.

        public int StepOrder { get; set; }
        public int? NextStepId { get; set; }
        public bool IsFinal { get; set; }
        public bool IsInitial { get; set; } // Marks the starting step

        public string? AllowedRoles { get; set; } // JSON array of roles
        public string? Conditions { get; set; } // JSON conditions
        public string? Configuration { get; set; } // JSON step-specific configuration
        public bool IsActive { get; set; } = true;

        public int? TimeoutHours { get; set; } // Step-specific timeout
        public bool RequiresComments { get; set; }
        public bool AllowDelegation { get; set; }

        // Navigation properties
        public Workflow? Workflow { get; set; }
        public ICollection<WorkflowTransition> FromTransitions { get; set; } = new List<WorkflowTransition>();
        public ICollection<WorkflowTransition> ToTransitions { get; set; } = new List<WorkflowTransition>();
    }

}
