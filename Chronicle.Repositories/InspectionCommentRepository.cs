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
    public class InspectionCommentRepository : DapperRepository<InspectionComment, int>, IInspectionCommentRepository
    {
        public InspectionCommentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionComments", "CommentID")
        {
        }

        public async Task<IEnumerable<InspectionComment>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE InspectionRequestID = @InspectionRequestID AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByCreatedByAsync(int createdBy)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE CreatedBy = @CreatedBy AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { CreatedBy = createdBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByCommentTypeAsync(string commentType)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE CommentType = @CommentType AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { CommentType = commentType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByPriorityAsync(string priority)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE Priority = @Priority AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { Priority = priority },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByStatusAsync(string status)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE Status = @Status AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { Status = status },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetInternalCommentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE InspectionRequestID = @InspectionRequestID AND IsInternal = 1 AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetExternalCommentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE InspectionRequestID = @InspectionRequestID AND IsInternal = 0 AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetCommentsRequiringResponseAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionComments 
                WHERE RequiresResponse = 1 
                AND RespondedDate IS NULL 
                AND IsActive = 1 
                ORDER BY ResponseDueDate ASC, CreatedDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetOverdueCommentsAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionComments 
                WHERE RequiresResponse = 1 
                AND ResponseDueDate < GETDATE() 
                AND RespondedDate IS NULL 
                AND IsActive = 1 
                ORDER BY ResponseDueDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetRepliesAsync(int parentCommentId)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE ParentCommentID = @ParentCommentID AND IsActive = 1 ORDER BY CreatedDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { ParentCommentID = parentCommentId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetRootCommentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE InspectionRequestID = @InspectionRequestID AND ParentCommentID IS NULL AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByRespondedByAsync(int respondedBy)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE RespondedBy = @RespondedBy AND IsActive = 1 ORDER BY RespondedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { RespondedBy = respondedBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionComment>> GetByCreatedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE CreatedDate >= @FromDate AND CreatedDate <= @ToDate AND IsActive = 1 ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionComment> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectionComments WHERE CommentID = @CommentID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionComment>(
                sql,
                new { CommentID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionComment inspectionComment)
        {
            const string sql = @"
                INSERT INTO InspectionComments (
                    InspectionRequestID, ParentCommentID, CommentType, Subject, CommentText, 
                    Priority, Status, IsInternal, RequiresResponse, ResponseDueDate, 
                    RespondedDate, RespondedBy, CreatedDate, CreatedBy, ModifiedDate, 
                    ModifiedBy, IsActive)
                VALUES (
                    @InspectionRequestID, @ParentCommentID, @CommentType, @Subject, @CommentText, 
                    @Priority, @Status, @IsInternal, @RequiresResponse, @ResponseDueDate, 
                    @RespondedDate, @RespondedBy, @CreatedDate, @CreatedBy, @ModifiedDate, 
                    @ModifiedBy, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (inspectionComment.CreatedDate == default(DateTime))
            {
                inspectionComment.CreatedDate = DateTime.UtcNow;
                inspectionComment.ModifiedDate = DateTime.UtcNow;
            }

            // Set default status if not set
            if (string.IsNullOrEmpty(inspectionComment.Status))
            {
                inspectionComment.Status = "Open";
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionComment,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionComment inspectionComment)
        {
            const string sql = @"
                UPDATE InspectionComments
                SET InspectionRequestID = @InspectionRequestID,
                    ParentCommentID = @ParentCommentID,
                    CommentType = @CommentType,
                    Subject = @Subject,
                    CommentText = @CommentText,
                    Priority = @Priority,
                    Status = @Status,
                    IsInternal = @IsInternal,
                    RequiresResponse = @RequiresResponse,
                    ResponseDueDate = @ResponseDueDate,
                    RespondedDate = @RespondedDate,
                    RespondedBy = @RespondedBy,
                    ModifiedDate = @ModifiedDate,
                    ModifiedBy = @ModifiedBy,
                    IsActive = @IsActive
                WHERE CommentID = @CommentID";

            // Set modification date
            inspectionComment.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionComment,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionComment>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectionComments ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionComment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionComment>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    Subject LIKE @SearchTerm OR 
                    CommentText LIKE @SearchTerm OR
                    CommentType LIKE @SearchTerm OR
                    Priority LIKE @SearchTerm OR
                    Status LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionComment>(
                "InspectionComments",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectionComments WHERE CommentID = @CommentID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { CommentID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
