using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class InspectionNotificationService : IInspectionNotificationService
    {
        private readonly IInspectionNotificationRepository _inspectionNotificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionNotificationService(
            IInspectionNotificationRepository inspectionNotificationRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionNotification>> GetInspectionNotificationByIdAsync(int notificationId)
        {
            try
            {
                var inspectionNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (inspectionNotification == null)
                {
                    return ServiceResult<InspectionNotification>.FailureResult("Inspection notification not found");
                }

                return ServiceResult<InspectionNotification>.SuccessResult(inspectionNotification);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionNotification>.FailureResult($"Error retrieving inspection notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByRecipientUserIdAsync(int recipientUserId)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByRecipientUserIdAsync(recipientUserId);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByNotificationTypeAsync(string notificationType)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByNotificationTypeAsync(notificationType);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByPriorityAsync(string priority)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByPriorityAsync(priority);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByDeliveryMethodAsync(string deliveryMethod)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByDeliveryMethodAsync(deliveryMethod);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetByDeliveryStatusAsync(string deliveryStatus)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetByDeliveryStatusAsync(deliveryStatus);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetUnreadNotificationsAsync(int recipientUserId)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetUnreadNotificationsAsync(recipientUserId);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving unread notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetReadNotificationsAsync(int recipientUserId)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetReadNotificationsAsync(recipientUserId);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving read notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetUnsentNotificationsAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetUnsentNotificationsAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving unsent notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetSentNotificationsAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetSentNotificationsAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving sent notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetFailedNotificationsAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetFailedNotificationsAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving failed notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetScheduledNotificationsAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetScheduledNotificationsAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving scheduled notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetExpiredNotificationsAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetExpiredNotificationsAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving expired notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetNotificationsDueForRetryAsync()
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetNotificationsDueForRetryAsync();
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving notifications due for retry: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetNotificationsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetNotificationsByDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionNotification>>> GetRecentNotificationsAsync(int recipientUserId, int days = 7)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetRecentNotificationsAsync(recipientUserId, days);
                return ServiceResult<IEnumerable<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionNotification>>.FailureResult($"Error retrieving recent notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetUnreadCountAsync(int recipientUserId)
        {
            try
            {
                var count = await _inspectionNotificationRepository.GetUnreadCountAsync(recipientUserId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error retrieving unread count: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionNotificationAsync(InspectionNotification inspectionNotification)
        {
            try
            {
                // Set default values
                inspectionNotification.CreatedDate = DateTime.UtcNow;
                inspectionNotification.IsRead = false;
                inspectionNotification.IsSent = false;
                inspectionNotification.RetryCount = 0;

                if (string.IsNullOrEmpty(inspectionNotification.Priority))
                {
                    inspectionNotification.Priority = "Normal";
                }

                if (string.IsNullOrEmpty(inspectionNotification.DeliveryStatus))
                {
                    inspectionNotification.DeliveryStatus = "Pending";
                }

                _unitOfWork.BeginTransaction();

                int notificationId = await _inspectionNotificationRepository.InsertAsync(inspectionNotification);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(notificationId, "Inspection notification created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> ScheduleNotificationAsync(InspectionNotification inspectionNotification, DateTime scheduledDate)
        {
            try
            {
                inspectionNotification.ScheduledDate = scheduledDate;
                inspectionNotification.CreatedDate = DateTime.UtcNow;
                inspectionNotification.IsRead = false;
                inspectionNotification.IsSent = false;
                inspectionNotification.RetryCount = 0;

                if (string.IsNullOrEmpty(inspectionNotification.Priority))
                {
                    inspectionNotification.Priority = "Normal";
                }

                if (string.IsNullOrEmpty(inspectionNotification.DeliveryStatus))
                {
                    inspectionNotification.DeliveryStatus = "Scheduled";
                }

                _unitOfWork.BeginTransaction();

                int notificationId = await _inspectionNotificationRepository.InsertAsync(inspectionNotification);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(notificationId, "Notification scheduled successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error scheduling notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionNotification inspectionNotification)
        {
            try
            {
                // Check if inspection notification exists
                var existingInspectionNotification = await _inspectionNotificationRepository.GetByIdAsync(inspectionNotification.NotificationID);
                if (existingInspectionNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(inspectionNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection notification updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                if (existingNotification.IsRead)
                {
                    return ServiceResult<bool>.FailureResult("Notification is already marked as read");
                }

                existingNotification.IsRead = true;
                existingNotification.ReadDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Notification marked as read successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error marking notification as read: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsUnreadAsync(int notificationId)
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                existingNotification.IsRead = false;
                existingNotification.ReadDate = null;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Notification marked as unread successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error marking notification as unread: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsSentAsync(int notificationId, string deliveryMethod, string deliveryStatus = "Delivered")
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                existingNotification.IsSent = true;
                existingNotification.SentDate = DateTime.UtcNow;
                existingNotification.DeliveryMethod = deliveryMethod;
                existingNotification.DeliveryStatus = deliveryStatus;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Notification marked as sent successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error marking notification as sent: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsFailedAsync(int notificationId, string failureReason)
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                existingNotification.DeliveryStatus = "Failed";
                existingNotification.RetryCount += 1;

                // Add failure reason to message for tracking
                if (!string.IsNullOrEmpty(failureReason))
                {
                    existingNotification.Message = $"{existingNotification.Message}\n\nDelivery Failed: {failureReason}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Notification marked as failed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error marking notification as failed: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RetryNotificationAsync(int notificationId)
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                if (existingNotification.DeliveryStatus != "Failed")
                {
                    return ServiceResult<bool>.FailureResult("Only failed notifications can be retried");
                }

                if (existingNotification.RetryCount >= 3)
                {
                    return ServiceResult<bool>.FailureResult("Maximum retry attempts reached");
                }

                if (existingNotification.ExpiryDate.HasValue && existingNotification.ExpiryDate < DateTime.UtcNow)
                {
                    return ServiceResult<bool>.FailureResult("Notification has expired and cannot be retried");
                }

                existingNotification.DeliveryStatus = "Pending";
                existingNotification.IsSent = false;
                existingNotification.SentDate = null;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Notification queued for retry successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error retrying notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelScheduledNotificationAsync(int notificationId)
        {
            try
            {
                var existingNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                if (existingNotification.IsSent)
                {
                    return ServiceResult<bool>.FailureResult("Cannot cancel notification that has already been sent");
                }

                existingNotification.DeliveryStatus = "Cancelled";
                existingNotification.ExpiryDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.UpdateAsync(existingNotification);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Scheduled notification cancelled successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error cancelling scheduled notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> MarkAllAsReadAsync(int recipientUserId)
        {
            try
            {
                var unreadNotifications = await _inspectionNotificationRepository.GetUnreadNotificationsAsync(recipientUserId);
                int updatedCount = 0;

                _unitOfWork.BeginTransaction();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.ReadDate = DateTime.UtcNow;

                    bool updated = await _inspectionNotificationRepository.UpdateAsync(notification);
                    if (updated) updatedCount++;
                }

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(updatedCount, $"{updatedCount} notifications marked as read successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error marking all notifications as read: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int notificationId,int tenantId)
        {
            try
            {
                // Check if inspection notification exists
                var existingInspectionNotification = await _inspectionNotificationRepository.GetByIdAsync(notificationId);
                if (existingInspectionNotification == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection notification not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionNotificationRepository.DeleteAsync(notificationId,tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection notification deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection notification: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionNotification>> GetInspectionNotificationsAsync()
        {
            return await _inspectionNotificationRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectionNotification>>> GetPagedInspectionNotificationsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectionNotifications = await _inspectionNotificationRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectionNotification>>.SuccessResult(inspectionNotifications);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionNotification>>.FailureResult($"Error retrieving inspection notifications: {ex.Message}");
            }
        }
    }
}
