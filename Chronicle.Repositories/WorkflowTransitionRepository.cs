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
    public class WorkflowTransitionRepository : DapperRepository<WorkflowTransition, int>, IWorkflowTransitionRepository
    {
        public WorkflowTransitionRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "WorkflowTransitions", "TransitionId")
        {
        }

        public async Task<WorkflowTransition> GetByIdAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.TransitionId = @TransitionId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowTransition>(
                sql,
                new { TransitionId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowTransition> GetByIdWithStepsAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wt.*, 
                       fs.StepId as FromStepId, fs.StepCode as FromStepCode, fs.StepName as FromStepName, 
                       fs.StepType as FromStepType, fs.StepOrder as FromStepOrder, fs.IsActive as FromStepIsActive,
                       ts.StepId as ToStepId, ts.StepCode as ToStepCode, ts.StepName as ToStepName, 
                       ts.StepType as ToStepType, ts.StepOrder as ToStepOrder, ts.IsActive as ToStepIsActive
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                LEFT JOIN WorkflowSteps fs ON wt.FromStepId = fs.StepId
                LEFT JOIN WorkflowSteps ts ON wt.ToStepId = ts.StepId
                WHERE wt.TransitionId = @TransitionId AND w.TenantID = @TenantID";

            var transition = await _unitOfWork.Connection.QueryAsync<WorkflowTransition, WorkflowStep, WorkflowStep, WorkflowTransition>(
                sql,
                (trans, fromStep, toStep) =>
                {
                    trans.FromStep = fromStep;
                    trans.ToStep = toStep;
                    return trans;
                },
                new { TransitionId = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "FromStepId,ToStepId");

            return transition.FirstOrDefault();
        }

        public async Task<WorkflowTransition> GetByActionCodeAsync(string actionCode, int fromStepId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.ActionCode = @ActionCode AND wt.FromStepId = @FromStepId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowTransition>(
                sql,
                new { ActionCode = actionCode, FromStepId = fromStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                ORDER BY wt.Priority DESC, wt.ActionName";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByFromStepIdAsync(int fromStepId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.FromStepId = @FromStepId AND wt.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY wt.Priority DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { FromStepId = fromStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByToStepIdAsync(int toStepId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.ToStepId = @ToStepId AND wt.IsActive = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { ToStepId = toStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetActiveTransitionsByWorkflowAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.WorkflowId = @WorkflowId AND wt.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY wt.Priority DESC, wt.ActionName";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetTransitionsByRoleAsync(string role, int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                AND wt.AllowedRoles LIKE @Role
                ORDER BY wt.Priority DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId, Role = $"%{role}%" },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetTransitionsRequiringApprovalAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.WorkflowId = @WorkflowId AND wt.RequiresApproval = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowTransition> GetHighestPriorityTransitionAsync(int fromStepId, int tenantId)
        {
            const string sql = @"
                SELECT TOP 1 wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.FromStepId = @FromStepId AND wt.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY wt.Priority DESC";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowTransition>(
                sql,
                new { FromStepId = fromStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<bool> TransitionExistsAsync(int fromStepId, int toStepId, string actionCode, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.FromStepId = @FromStepId AND wt.ToStepId = @ToStepId 
                AND wt.ActionCode = @ActionCode AND w.TenantID = @TenantID";

            var count = await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { FromStepId = fromStepId, ToStepId = toStepId, ActionCode = actionCode, TenantID = tenantId },
                _unitOfWork.Transaction);

            return count > 0;
        }

        public override async Task<int> InsertAsync(WorkflowTransition workflowTransition)
        {
            const string sql = @"
                INSERT INTO WorkflowTransitions (
                    WorkflowId, FromStepId, ToStepId, ActionCode, ActionName,
                    Condition, AllowedRoles, IsActive, Priority, Configuration,
                    RequiresApproval, RequiresComments, NotificationRoles)
                VALUES (
                    @WorkflowId, @FromStepId, @ToStepId, @ActionCode, @ActionName,
                    @Condition, @AllowedRoles, @IsActive, @Priority, @Configuration,
                    @RequiresApproval, @RequiresComments, @NotificationRoles);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflowTransition,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowTransition workflowTransition)
        {
            const string sql = @"
                UPDATE WorkflowTransitions
                SET FromStepId = @FromStepId,
                    ToStepId = @ToStepId,
                    ActionCode = @ActionCode,
                    ActionName = @ActionName,
                    Condition = @Condition,
                    AllowedRoles = @AllowedRoles,
                    IsActive = @IsActive,
                    Priority = @Priority,
                    Configuration = @Configuration,
                    RequiresApproval = @RequiresApproval,
                    RequiresComments = @RequiresComments,
                    NotificationRoles = @NotificationRoles
                WHERE TransitionId = @TransitionId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflowTransition,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkflowTransition>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT wt.* 
                FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowTransition>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = @"wt.TransitionId IN (
                SELECT wt2.TransitionId FROM WorkflowTransitions wt2
                INNER JOIN Workflows w ON wt2.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID)";

            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    wt.ActionName LIKE @SearchTerm OR 
                    wt.ActionCode LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowTransition>(
                "WorkflowTransitions wt",
                "wt.Priority DESC, wt.ActionName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = @"
                DELETE wt FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.TransitionId = @TransitionId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { TransitionId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                DELETE wt FROM WorkflowTransitions wt
                INNER JOIN Workflows w ON wt.WorkflowId = w.WorkflowId
                WHERE wt.WorkflowId = @WorkflowId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
