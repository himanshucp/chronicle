using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionDocument
    {
        public int DocumentID { get; set; }
        public int InspectionRequestID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentCategory { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentUrl { get; set; }
        public long? FileSize { get; set; }
        public string MimeType { get; set; }
        public string DocumentVersion { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public DateTime UploadedDate { get; set; }
        public int UploadedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
