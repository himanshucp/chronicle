using Chronicle.Data.Extensions;
using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public class InspectionNotificationRepository : DapperRepository<InspectionNotification, int>, IInspectionNotificationRepository
    {
        public InspectionNotificationRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionNotifications", "NotificationID")
        {
        }

        public async Task<IEnumerable<InspectionNotification>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE InspectionRequestID = @InspectionRequestID ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetByRecipientUserIdAsync(int recipientUserId)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE RecipientUserID = @RecipientUserID ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { RecipientUserID = recipientUserId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetByNotificationTypeAsync(string notificationType)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE NotificationType = @NotificationType ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { NotificationType = notificationType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetByPriorityAsync(string priority)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE Priority = @Priority ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { Priority = priority },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetByDeliveryMethodAsync(string deliveryMethod)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE DeliveryMethod = @DeliveryMethod ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { DeliveryMethod = deliveryMethod },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetByDeliveryStatusAsync(string deliveryStatus)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE DeliveryStatus = @DeliveryStatus ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { DeliveryStatus = deliveryStatus },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetUnreadNotificationsAsync(int recipientUserId)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE RecipientUserID = @RecipientUserID AND IsRead = 0 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { RecipientUserID = recipientUserId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetReadNotificationsAsync(int recipientUserId)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE RecipientUserID = @RecipientUserID AND IsRead = 1 ORDER BY ReadDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { RecipientUserID = recipientUserId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetUnsentNotificationsAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionNotifications 
                WHERE IsSent = 0 
                AND (ScheduledDate IS NULL OR ScheduledDate <= GETDATE())
                AND (ExpiryDate IS NULL OR ExpiryDate > GETDATE())
                ORDER BY Priority DESC, CreatedDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetSentNotificationsAsync()
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE IsSent = 1 ORDER BY SentDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetFailedNotificationsAsync()
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE DeliveryStatus = 'Failed' ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetScheduledNotificationsAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionNotifications 
                WHERE ScheduledDate IS NOT NULL 
                AND ScheduledDate > GETDATE() 
                AND IsSent = 0
                ORDER BY ScheduledDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetExpiredNotificationsAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionNotifications 
                WHERE ExpiryDate IS NOT NULL 
                AND ExpiryDate < GETDATE() 
                AND IsSent = 0
                ORDER BY ExpiryDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetNotificationsDueForRetryAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionNotifications 
                WHERE DeliveryStatus = 'Failed' 
                AND RetryCount < 3 
                AND (ExpiryDate IS NULL OR ExpiryDate > GETDATE())
                ORDER BY CreatedDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetNotificationsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE CreatedDate >= @FromDate AND CreatedDate <= @ToDate ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionNotification>> GetRecentNotificationsAsync(int recipientUserId, int days = 7)
        {
            const string sql = @"
                SELECT * FROM InspectionNotifications 
                WHERE RecipientUserID = @RecipientUserID 
                AND CreatedDate >= @FromDate 
                ORDER BY CreatedDate DESC";

            var fromDate = DateTime.UtcNow.AddDays(-days);
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                new { RecipientUserID = recipientUserId, FromDate = fromDate },
                _unitOfWork.Transaction);
        }

        public async Task<int> GetUnreadCountAsync(int recipientUserId)
        {
            const string sql = "SELECT COUNT(*) FROM InspectionNotifications WHERE RecipientUserID = @RecipientUserID AND IsRead = 0";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { RecipientUserID = recipientUserId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionNotification> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectionNotifications WHERE NotificationID = @NotificationID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionNotification>(
                sql,
                new { NotificationID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionNotification inspectionNotification)
        {
            const string sql = @"
                INSERT INTO InspectionNotifications (
                    InspectionRequestID, RecipientUserID, NotificationType, Subject, Message, 
                    Priority, IsRead, IsSent, SentDate, ReadDate, DeliveryMethod, 
                    DeliveryStatus, RetryCount, CreatedDate, ScheduledDate, ExpiryDate)
                VALUES (
                    @InspectionRequestID, @RecipientUserID, @NotificationType, @Subject, @Message, 
                    @Priority, @IsRead, @IsSent, @SentDate, @ReadDate, @DeliveryMethod, 
                    @DeliveryStatus, @RetryCount, @CreatedDate, @ScheduledDate, @ExpiryDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (inspectionNotification.CreatedDate == default(DateTime))
            {
                inspectionNotification.CreatedDate = DateTime.UtcNow;
            }

            // Set default values
            if (string.IsNullOrEmpty(inspectionNotification.Priority))
            {
                inspectionNotification.Priority = "Normal";
            }

            if (string.IsNullOrEmpty(inspectionNotification.DeliveryStatus))
            {
                inspectionNotification.DeliveryStatus = "Pending";
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionNotification,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionNotification inspectionNotification)
        {
            const string sql = @"
                UPDATE InspectionNotifications
                SET InspectionRequestID = @InspectionRequestID,
                    RecipientUserID = @RecipientUserID,
                    NotificationType = @NotificationType,
                    Subject = @Subject,
                    Message = @Message,
                    Priority = @Priority,
                    IsRead = @IsRead,
                    IsSent = @IsSent,
                    SentDate = @SentDate,
                    ReadDate = @ReadDate,
                    DeliveryMethod = @DeliveryMethod,
                    DeliveryStatus = @DeliveryStatus,
                    RetryCount = @RetryCount,
                    ScheduledDate = @ScheduledDate,
                    ExpiryDate = @ExpiryDate
                WHERE NotificationID = @NotificationID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionNotification,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionNotification>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectionNotifications ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionNotification>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionNotification>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    Subject LIKE @SearchTerm OR 
                    Message LIKE @SearchTerm OR
                    NotificationType LIKE @SearchTerm OR
                    Priority LIKE @SearchTerm OR
                    DeliveryMethod LIKE @SearchTerm OR
                    DeliveryStatus LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionNotification>(
                "InspectionNotifications",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectionNotifications WHERE NotificationID = @NotificationID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { NotificationID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
