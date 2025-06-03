using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectorAssignment
    {
        public int AssignmentID { get; set; }
        public int InspectionRequestID { get; set; }
        public int InspectorUserID { get; set; }
        public string AssignmentType { get; set; }
        public DateTime AssignedDate { get; set; }
        public int AssignedBy { get; set; }
        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public string Status { get; set; }
        public DateTime? AcceptanceDate { get; set; }
        public string DeclineReason { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}
