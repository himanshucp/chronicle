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
    public class InspectionActionRepository : DapperRepository<InspectionAction, int>, IInspectionActionRepository
    {
        public InspectionActionRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionActions", "ActionID")
        {
        }

        public async Task<IEnumerable<InspectionAction>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE InspectionRequestID = @InspectionRequestID ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByInspectorUserIdAsync(int inspectorUserId)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE InspectorUserID = @InspectorUserID ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { InspectorUserID = inspectorUserId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByActionTypeAsync(string actionType)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ActionType = @ActionType ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { ActionType = actionType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByInspectionResultAsync(string inspectionResult)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE InspectionResult = @InspectionResult ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { InspectionResult = inspectionResult },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByComplianceStatusAsync(string complianceStatus)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ComplianceStatus = @ComplianceStatus ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { ComplianceStatus = complianceStatus },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByActionDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ActionDate >= @FromDate AND ActionDate <= @ToDate ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetPendingActionsAsync()
        {
            const string sql = "SELECT * FROM InspectionActions WHERE IsCompleted = 0 ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetCompletedActionsAsync()
        {
            const string sql = "SELECT * FROM InspectionActions WHERE IsCompleted = 1 ORDER BY CompletedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetActionsRequiringFollowUpAsync()
        {
            const string sql = "SELECT * FROM InspectionActions WHERE FollowUpRequired = 1 AND (FollowUpDate IS NULL OR FollowUpDate <= GETDATE()) ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetActionsRequiringReviewAsync()
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ReviewRequired = 1 AND ReviewedBy IS NULL ORDER BY ActionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionAction>> GetByReviewedByAsync(int reviewedBy)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ReviewedBy = @ReviewedBy ORDER BY ReviewedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                new { ReviewedBy = reviewedBy },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionAction> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectionActions WHERE ActionID = @ActionID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionAction>(
                sql,
                new { ActionID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionAction inspectionAction)
        {
            const string sql = @"
                INSERT INTO InspectionActions (
                    InspectionRequestID, InspectorUserID, ActionDate, ActionType, InspectionResult, 
                    ComplianceStatus, CompletionPercentage, QualityRating, SafetyCompliance, 
                    EnvironmentalCompliance, Comments, Recommendations, NextAction, FollowUpRequired, 
                    FollowUpDate, IsCompleted, CompletedDate, ReviewRequired, ReviewedBy, 
                    ReviewedDate, ReviewComments, CreatedDate, ModifiedDate)
                VALUES (
                    @InspectionRequestID, @InspectorUserID, @ActionDate, @ActionType, @InspectionResult, 
                    @ComplianceStatus, @CompletionPercentage, @QualityRating, @SafetyCompliance, 
                    @EnvironmentalCompliance, @Comments, @Recommendations, @NextAction, @FollowUpRequired, 
                    @FollowUpDate, @IsCompleted, @CompletedDate, @ReviewRequired, @ReviewedBy, 
                    @ReviewedDate, @ReviewComments, @CreatedDate, @ModifiedDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (inspectionAction.CreatedDate == default(DateTime))
            {
                inspectionAction.CreatedDate = DateTime.UtcNow;
                inspectionAction.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionAction,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionAction inspectionAction)
        {
            const string sql = @"
                UPDATE InspectionActions
                SET InspectionRequestID = @InspectionRequestID,
                    InspectorUserID = @InspectorUserID,
                    ActionDate = @ActionDate,
                    ActionType = @ActionType,
                    InspectionResult = @InspectionResult,
                    ComplianceStatus = @ComplianceStatus,
                    CompletionPercentage = @CompletionPercentage,
                    QualityRating = @QualityRating,
                    SafetyCompliance = @SafetyCompliance,
                    EnvironmentalCompliance = @EnvironmentalCompliance,
                    Comments = @Comments,
                    Recommendations = @Recommendations,
                    NextAction = @NextAction,
                    FollowUpRequired = @FollowUpRequired,
                    FollowUpDate = @FollowUpDate,
                    IsCompleted = @IsCompleted,
                    CompletedDate = @CompletedDate,
                    ReviewRequired = @ReviewRequired,
                    ReviewedBy = @ReviewedBy,
                    ReviewedDate = @ReviewedDate,
                    ReviewComments = @ReviewComments,
                    ModifiedDate = @ModifiedDate
                WHERE ActionID = @ActionID";

            // Set modification date
            inspectionAction.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionAction,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionAction>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectionActions ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionAction>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionAction>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    ActionType LIKE @SearchTerm OR 
                    InspectionResult LIKE @SearchTerm OR
                    ComplianceStatus LIKE @SearchTerm OR
                    Comments LIKE @SearchTerm OR
                    Recommendations LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionAction>(
                "InspectionActions",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectionActions WHERE ActionID = @ActionID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { ActionID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
