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

        public async Task<WorkflowHistory> GetByIdAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.HistoryId = @HistoryId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowHistory>(
                sql,
                new { HistoryId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowHistory> GetByIdWithStepsAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wh.*, 
                       fs.StepId as FromStepId, fs.StepCode as FromStepCode, fs.StepName as FromStepName,
                       ts.StepId as ToStepId, ts.StepCode as ToStepCode, ts.StepName as ToStepName
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                LEFT JOIN WorkflowSteps fs ON wh.FromStepId = fs.StepId
                LEFT JOIN WorkflowSteps ts ON wh.ToStepId = ts.StepId
                WHERE wh.HistoryId = @HistoryId AND w.TenantID = @TenantID";

            var history = await _unitOfWork.Connection.QueryAsync<WorkflowHistory, WorkflowStep, WorkflowStep, WorkflowHistory>(
                sql,
                (hist, fromStep, toStep) =>
                {
                    hist.FromStep = fromStep;
                    hist.ToStep = toStep;
                    return hist;
                },
                new { HistoryId = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "FromStepId,ToStepId");

            return history.FirstOrDefault();
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByInstanceIdAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.InstanceId = @InstanceId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByActionByAsync(string actionBy, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.ActionBy = @ActionBy AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { ActionBy = actionBy, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByActionCodeAsync(string actionCode, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.ActionCode = @ActionCode AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { ActionCode = actionCode, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByFromStepAsync(int fromStepId, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.FromStepId = @FromStepId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { FromStepId = fromStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByToStepAsync(int toStepId, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.ToStepId = @ToStepId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { ToStepId = toStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.TransitionDate >= @StartDate AND wh.TransitionDate <= @EndDate 
                AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { StartDate = startDate, EndDate = endDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByAssignedToAsync(string assignedTo, int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.AssignedTo = @AssignedTo AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { AssignedTo = assignedTo, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowHistory> GetLastTransitionAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                SELECT TOP 1 wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.InstanceId = @InstanceId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowHistory>(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<double> GetAverageStepDurationAsync(int fromStepId, int toStepId, int tenantId)
        {
            const string sql = @"
                SELECT AVG(wh.DurationMinutes)
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.FromStepId = @FromStepId AND wh.ToStepId = @ToStepId 
                AND wh.DurationMinutes IS NOT NULL AND w.TenantID = @TenantID";

            var result = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<double?>(
                sql,
                new { FromStepId = fromStepId, ToStepId = toStepId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return result ?? 0;
        }

        public async Task<int> GetTransitionCountAsync(int fromStepId, int toStepId, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.FromStepId = @FromStepId AND wh.ToStepId = @ToStepId 
                AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { FromStepId = fromStepId, ToStepId = toStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowHistory workflowHistory)
        {
            const string sql = @"
                INSERT INTO WorkflowHistory (
                    InstanceId, FromStepId, ToStepId, ActionCode, Action,
                    Comments, TransitionDate, ActionBy, ActionByRole, AdditionalData,
                    DurationMinutes, AssignedTo, DueDate)
                VALUES (
                    @InstanceId, @FromStepId, @ToStepId, @ActionCode, @Action,
                    @Comments, @TransitionDate, @ActionBy, @ActionByRole, @AdditionalData,
                    @DurationMinutes, @AssignedTo, @DueDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set transition date if not set
            if (workflowHistory.TransitionDate == default(DateTime))
            {
                workflowHistory.TransitionDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflowHistory,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowHistory workflowHistory)
        {
            const string sql = @"
                UPDATE WorkflowHistory
                SET Comments = @Comments,
                    ActionByRole = @ActionByRole,
                    AdditionalData = @AdditionalData,
                    DurationMinutes = @DurationMinutes,
                    AssignedTo = @AssignedTo,
                    DueDate = @DueDate
                WHERE HistoryId = @HistoryId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflowHistory,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkflowHistory>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT wh.* 
                FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowHistory>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowHistory>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = @"wh.HistoryId IN (
                SELECT wh2.HistoryId FROM WorkflowHistory wh2
                INNER JOIN WorkflowInstances wi ON wh2.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID)";

            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    wh.Action LIKE @SearchTerm OR 
                    wh.ActionCode LIKE @SearchTerm OR
                    wh.ActionBy LIKE @SearchTerm OR
                    wh.Comments LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowHistory>(
                "WorkflowHistory wh",
                "wh.TransitionDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = @"
                DELETE wh FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.HistoryId = @HistoryId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { HistoryId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByInstanceIdAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                DELETE wh FROM WorkflowHistory wh
                INNER JOIN WorkflowInstances wi ON wh.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wh.InstanceId = @InstanceId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
