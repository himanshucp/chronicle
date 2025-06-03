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
    public class InspectorAssignmentRepository : DapperRepository<InspectorAssignment, int>, IInspectorAssignmentRepository
    {
        public InspectorAssignmentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectorAssignments", "AssignmentID")
        {
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE InspectionRequestID = @InspectionRequestID AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByInspectorUserIdAsync(int inspectorUserId)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE InspectorUserID = @InspectorUserID AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { InspectorUserID = inspectorUserId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByAssignedByAsync(int assignedBy)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE AssignedBy = @AssignedBy AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { AssignedBy = assignedBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByAssignmentTypeAsync(string assignmentType)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE AssignmentType = @AssignmentType AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { AssignmentType = assignmentType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByStatusAsync(string status)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE Status = @Status AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { Status = status },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetByAssignedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE AssignedDate >= @FromDate AND AssignedDate <= @ToDate AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetPendingAssignmentsAsync()
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE Status = 'Pending' AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetAcceptedAssignmentsAsync()
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE Status = 'Accepted' AND IsActive = 1 ORDER BY AcceptanceDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetDeclinedAssignmentsAsync()
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE Status = 'Declined' AND IsActive = 1 ORDER BY ModifiedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetCompletedAssignmentsAsync()
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE Status = 'Completed' AND IsActive = 1 ORDER BY ActualCompletionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetOverdueAssignmentsAsync()
        {
            const string sql = @"
                SELECT * FROM InspectorAssignments 
                WHERE ExpectedCompletionDate < GETDATE() 
                AND Status NOT IN ('Completed', 'Declined') 
                AND IsActive = 1 
                ORDER BY ExpectedCompletionDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectorAssignment>> GetActiveAssignmentsForInspectorAsync(int inspectorUserId)
        {
            const string sql = @"
                SELECT * FROM InspectorAssignments 
                WHERE InspectorUserID = @InspectorUserID 
                AND Status IN ('Pending', 'Accepted', 'In Progress') 
                AND IsActive = 1 
                ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                new { InspectorUserID = inspectorUserId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectorAssignment> GetActiveAssignmentForInspectionRequestAsync(int inspectionRequestId)
        {
            const string sql = @"
                SELECT TOP 1 * FROM InspectorAssignments 
                WHERE InspectionRequestID = @InspectionRequestID 
                AND Status NOT IN ('Completed', 'Declined') 
                AND IsActive = 1 
                ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectorAssignment>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectorAssignment> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectorAssignments WHERE AssignmentID = @AssignmentID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectorAssignment>(
                sql,
                new { AssignmentID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectorAssignment inspectorAssignment)
        {
            const string sql = @"
                INSERT INTO InspectorAssignments (
                    InspectionRequestID, InspectorUserID, AssignmentType, AssignedDate, AssignedBy, 
                    ExpectedStartDate, ActualStartDate, ExpectedCompletionDate, ActualCompletionDate, 
                    Status, AcceptanceDate, DeclineReason, Notes, IsActive, CreatedDate, ModifiedDate)
                VALUES (
                    @InspectionRequestID, @InspectorUserID, @AssignmentType, @AssignedDate, @AssignedBy, 
                    @ExpectedStartDate, @ActualStartDate, @ExpectedCompletionDate, @ActualCompletionDate, 
                    @Status, @AcceptanceDate, @DeclineReason, @Notes, @IsActive, @CreatedDate, @ModifiedDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (inspectorAssignment.CreatedDate == default(DateTime))
            {
                inspectorAssignment.CreatedDate = DateTime.UtcNow;
                inspectorAssignment.ModifiedDate = DateTime.UtcNow;
            }

            // Set assigned date if not set
            if (inspectorAssignment.AssignedDate == default(DateTime))
            {
                inspectorAssignment.AssignedDate = DateTime.UtcNow;
            }

            // Set default status if not set
            if (string.IsNullOrEmpty(inspectorAssignment.Status))
            {
                inspectorAssignment.Status = "Pending";
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectorAssignment,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectorAssignment inspectorAssignment)
        {
            const string sql = @"
                UPDATE InspectorAssignments
                SET InspectionRequestID = @InspectionRequestID,
                    InspectorUserID = @InspectorUserID,
                    AssignmentType = @AssignmentType,
                    AssignedDate = @AssignedDate,
                    AssignedBy = @AssignedBy,
                    ExpectedStartDate = @ExpectedStartDate,
                    ActualStartDate = @ActualStartDate,
                    ExpectedCompletionDate = @ExpectedCompletionDate,
                    ActualCompletionDate = @ActualCompletionDate,
                    Status = @Status,
                    AcceptanceDate = @AcceptanceDate,
                    DeclineReason = @DeclineReason,
                    Notes = @Notes,
                    IsActive = @IsActive,
                    ModifiedDate = @ModifiedDate
                WHERE AssignmentID = @AssignmentID";

            // Set modification date
            inspectorAssignment.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectorAssignment,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectorAssignment>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectorAssignments ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectorAssignment>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectorAssignment>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    AssignmentType LIKE @SearchTerm OR 
                    Status LIKE @SearchTerm OR
                    DeclineReason LIKE @SearchTerm OR
                    Notes LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectorAssignment>(
                "InspectorAssignments",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectorAssignments WHERE AssignmentID = @AssignmentID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { AssignmentID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
