using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionNotificationRepository : IRepository<InspectionNotification, int>
    {
        Task<IEnumerable<InspectionNotification>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionNotification>> GetByRecipientUserIdAsync(int recipientUserId);
        Task<IEnumerable<InspectionNotification>> GetByNotificationTypeAsync(string notificationType);
        Task<IEnumerable<InspectionNotification>> GetByPriorityAsync(string priority);
        Task<IEnumerable<InspectionNotification>> GetByDeliveryMethodAsync(string deliveryMethod);
        Task<IEnumerable<InspectionNotification>> GetByDeliveryStatusAsync(string deliveryStatus);
        Task<IEnumerable<InspectionNotification>> GetUnreadNotificationsAsync(int recipientUserId);
        Task<IEnumerable<InspectionNotification>> GetReadNotificationsAsync(int recipientUserId);
        Task<IEnumerable<InspectionNotification>> GetUnsentNotificationsAsync();
        Task<IEnumerable<InspectionNotification>> GetSentNotificationsAsync();
        Task<IEnumerable<InspectionNotification>> GetFailedNotificationsAsync();
        Task<IEnumerable<InspectionNotification>> GetScheduledNotificationsAsync();
        Task<IEnumerable<InspectionNotification>> GetExpiredNotificationsAsync();
        Task<IEnumerable<InspectionNotification>> GetNotificationsDueForRetryAsync();
        Task<IEnumerable<InspectionNotification>> GetNotificationsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectionNotification>> GetRecentNotificationsAsync(int recipientUserId, int days = 7);
        Task<int> GetUnreadCountAsync(int recipientUserId);
        Task<InspectionNotification> GetByIdAsync(int id);
        Task<IEnumerable<InspectionNotification>> GetAllAsync();
        Task<PagedResult<InspectionNotification>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
