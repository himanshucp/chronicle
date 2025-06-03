using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionRequest
    {
        public int InspectionRequestID { get; set; }
        public int TenantID { get; set; }
        public string IRNumber { get; set; }
        public int ContractID { get; set; }
        public int IssuerUserID { get; set; }
        public int IssuingCompanyID { get; set; }
        public int? RecipientCompanyID { get; set; }
        public int? RecipientPMCID { get; set; }
        public int? RecipientClientID { get; set; }
        public int DisciplineID { get; set; }
        public int? SubDisciplineID { get; set; }
        public string Location { get; set; }
        public string AreaUnit { get; set; }
        public string ChainageFrom { get; set; }
        public string ChainageTo { get; set; }
        public string FloorLevel { get; set; }
        public string AssetTagNumber { get; set; }
        public string SystemRoom { get; set; }
        public string SubsystemRoom { get; set; }
        public int? EarthworksLayerNumber { get; set; }
        public string BOQReferenceNumber { get; set; }
        public string WBSCode { get; set; }
        public string SnagNumbers { get; set; }
        public bool IsSnagWalkthrough { get; set; }
        public bool IsDesnaggingInspection { get; set; }
        public string InspectionDescription { get; set; }
        public int? ITPID { get; set; }
        public int? ActivityID { get; set; }
        public string InspectionInterventionPMC { get; set; }
        public string InspectionInterventionClient { get; set; }
        public string PreviousIRNumbers { get; set; }
        public DateTime InspectionDate { get; set; }
        public TimeSpan InspectionTime { get; set; }
        public string ExpectedDuration { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string Location3D { get; set; }
        public string GPSCoordinates { get; set; }
        public int? WorkflowInstanceID { get; set; }
        public string CurrentStatus { get; set; }
        public string PersonResponsible { get; set; }
        public int? SubmissionApprovedBy { get; set; }
        public DateTime? SubmissionApprovedDate { get; set; }
        public string SubmissionComments { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }

}
