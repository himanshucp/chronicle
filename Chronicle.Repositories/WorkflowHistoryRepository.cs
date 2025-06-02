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
    public class WorkflowHistoryRepository : DapperRepository<WorkflowHistory, int>, IWorkflowHistoryRepository
    {
        public WorkflowHistoryRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "WorkflowHistory", "HistoryId")
        {
        }

        public async Task<WorkflowHistory> GetByIdAsync(int historyId)
        {
            const string sql = "SELECT * FROM WorkflowHistory WHERE HistoryId = @HistoryId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowHistory>(
                sql,
                new { HistoryId = historyId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByInstanceIdAsync(int instanceId)
        {
            const string sql = "SELECT * FROM WorkflowHistory WHERE InstanceId = @InstanceId ORDER BY TransitionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { InstanceId = instanceId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByActionByAsync(string actionBy)
        {
            const string sql = "SELECT * FROM WorkflowHistory WHERE ActionBy = @ActionBy ORDER BY TransitionDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { ActionBy = actionBy },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowHistory>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    ActionCode LIKE @SearchTerm OR 
                    Action LIKE @SearchTerm OR
                    ActionBy LIKE @SearchTerm OR
                    ActionByRole LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowHistory>(
                "WorkflowHistory",
                "TransitionDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowHistory history)
        {
            const string sql = @"
                INSERT INTO WorkflowHistory (
                    InstanceId, FromStepId, ToStepId, ActionCode, Action, Comments, TransitionDate,
                    ActionBy, ActionByRole, AdditionalData, DurationMinutes, AssignedTo, DueDate)
                VALUES (
                    @InstanceId, @FromStepId, @ToStepId, @ActionCode, @Action, @Comments, @TransitionDate,
                    @ActionBy, @ActionByRole, @AdditionalData, @DurationMinutes, @AssignedTo, @DueDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            if (history.TransitionDate == default)
            {
                history.TransitionDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                history,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowHistory history)
        {
            const string sql = @"
                UPDATE WorkflowHistory
                SET Comments = @Comments,
                    AdditionalData = @AdditionalData,
                    DurationMinutes = @DurationMinutes
                WHERE HistoryId = @HistoryId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                history,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
