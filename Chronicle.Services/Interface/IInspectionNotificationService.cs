using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionNotificationService
    {
        Task<ServiceResult<InspectionNotification>> GetInspectionNotificationByIdAsync(int notificationId);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByRecipientUserIdAsync(int recipientUserId);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByNotificationTypeAsync(string notificationType);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByPriorityAsync(string priority);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByDeliveryMethodAsync(string deliveryMethod);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByDeliveryStatusAsync(string deliveryStatus);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetUnreadNotificationsAsync(int recipientUserId);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetReadNotificationsAsync(int recipientUserId);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetUnsentNotificationsAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetSentNotificationsAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetFailedNotificationsAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetScheduledNotificationsAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetExpiredNotificationsAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetNotificationsDueForRetryAsync();
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetNotificationsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ServiceResult<IEnumerable<InspectionNotification>>> GetRecentNotificationsAsync(int recipientUserId, int days = 7);
        Task<ServiceResult<int>> GetUnreadCountAsync(int recipientUserId);
        Task<IEnumerable<InspectionNotification>> GetInspectionNotificationsAsync();
        Task<ServiceResult<PagedResult<InspectionNotification>>> GetPagedInspectionNotificationsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionNotificationAsync(InspectionNotification inspectionNotification);
        Task<ServiceResult<int>> ScheduleNotificationAsync(InspectionNotification inspectionNotification, DateTime scheduledDate);
        Task<ServiceResult<bool>> UpdateAsync(InspectionNotification inspectionNotification);
        Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId);
        Task<ServiceResult<bool>> MarkAsUnreadAsync(int notificationId);
        Task<ServiceResult<bool>> MarkAsSentAsync(int notificationId, string deliveryMethod, string deliveryStatus = "Delivered");
        Task<ServiceResult<bool>> MarkAsFailedAsync(int notificationId, string failureReason);
        Task<ServiceResult<bool>> RetryNotificationAsync(int notificationId);
        Task<ServiceResult<bool>> CancelScheduledNotificationAsync(int notificationId);
        Task<ServiceResult<int>> MarkAllAsReadAsync(int recipientUserId);
        Task<ServiceResult<bool>> DeleteAsync(int notificationId,int tenantId);
    }
}
