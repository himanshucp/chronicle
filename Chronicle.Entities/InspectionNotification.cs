using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Entities
{
    public class InspectionNotification
    {
        public int NotificationID { get; set; }
        public int InspectionRequestID { get; set; }
        public int RecipientUserID { get; set; }
        public string NotificationType { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public bool IsRead { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryStatus { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
