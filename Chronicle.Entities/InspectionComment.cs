using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionComment
    {
        public int CommentID { get; set; }
        public int InspectionRequestID { get; set; }
        public int? ParentCommentID { get; set; }
        public string CommentType { get; set; }
        public string Subject { get; set; }
        public string CommentText { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public bool IsInternal { get; set; }
        public bool RequiresResponse { get; set; }
        public DateTime? ResponseDueDate { get; set; }
        public DateTime? RespondedDate { get; set; }
        public int? RespondedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }

}
