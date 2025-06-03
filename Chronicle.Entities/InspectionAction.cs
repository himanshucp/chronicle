using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionAction
    {
        public int ActionID { get; set; }
        public int InspectionRequestID { get; set; }
        public int InspectorUserID { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionType { get; set; }
        public string InspectionResult { get; set; }
        public string ComplianceStatus { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public int? QualityRating { get; set; }
        public bool? SafetyCompliance { get; set; }
        public bool? EnvironmentalCompliance { get; set; }
        public string Comments { get; set; }
        public string Recommendations { get; set; }
        public string NextAction { get; set; }
        public bool FollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool ReviewRequired { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string ReviewComments { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
