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
    public class WorkflowStepRepository : DapperRepository<WorkflowStep, int>, IWorkflowStepRepository
    {
        public WorkflowStepRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "WorkflowSteps", "StepId")
        {
        }

        public async Task<WorkflowStep> GetByIdAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.StepId = @StepId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { StepId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowStep> GetByIdWithTransitionsAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT ws.*, wt.*
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                LEFT JOIN WorkflowTransitions wt ON ws.StepId = wt.FromStepId OR ws.StepId = wt.ToStepId
                WHERE ws.StepId = @StepId AND w.TenantID = @TenantID";

            var stepDictionary = new Dictionary<int, WorkflowStep>();

            var steps = await _unitOfWork.Connection.QueryAsync<WorkflowStep, WorkflowTransition, WorkflowStep>(
                sql,
                (step, transition) =>
                {
                    if (!stepDictionary.TryGetValue(step.StepId, out WorkflowStep stepEntry))
                    {
                        stepEntry = step;
                        stepEntry.FromTransitions = new List<WorkflowTransition>();
                        stepEntry.ToTransitions = new List<WorkflowTransition>();
                        stepDictionary.Add(step.StepId, stepEntry);
                    }

                    if (transition != null)
                    {
                        if (transition.FromStepId == step.StepId)
                        {
                            stepEntry.FromTransitions.Add(transition);
                        }
                        else if (transition.ToStepId == step.StepId)
                        {
                            stepEntry.ToTransitions.Add(transition);
                        }
                    }

                    return stepEntry;
                },
                new { StepId = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "TransitionId");

            return steps.FirstOrDefault();
        }

        public async Task<WorkflowStep> GetByStepCodeAsync(string stepCode, int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.StepCode = @StepCode AND ws.WorkflowId = @WorkflowId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { StepCode = stepCode, WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                ORDER BY ws.StepOrder";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetByStepTypeAsync(string stepType, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.StepType = @StepType AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { StepType = stepType, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowStep> GetInitialStepAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND ws.IsInitial = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetFinalStepsAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND ws.IsFinal = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowStep> GetNextStepAsync(int currentStepId, int tenantId)
        {
            const string sql = @"
                SELECT ws2.* 
                FROM WorkflowSteps ws1
                INNER JOIN Workflows w ON ws1.WorkflowId = w.WorkflowId
                INNER JOIN WorkflowSteps ws2 ON ws1.NextStepId = ws2.StepId
                WHERE ws1.StepId = @CurrentStepId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { CurrentStepId = currentStepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetActiveStepsByWorkflowAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND ws.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY ws.StepOrder";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetStepsByRoleAsync(string role, int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                AND ws.AllowedRoles LIKE @Role
                ORDER BY ws.StepOrder";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId, Role = $"%{role}%" },
                _unitOfWork.Transaction);
        }

        public async Task<int> GetMaxStepOrderAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT ISNULL(MAX(ws.StepOrder), 0)
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowStep workflowStep)
        {
            const string sql = @"
                INSERT INTO WorkflowSteps (
                    WorkflowId, StepCode, StepName, StepType, StepOrder, NextStepId,
                    IsFinal, IsInitial, AllowedRoles, Conditions, Configuration, IsActive,
                    TimeoutHours, RequiresComments, AllowDelegation)
                VALUES (
                    @WorkflowId, @StepCode, @StepName, @StepType, @StepOrder, @NextStepId,
                    @IsFinal, @IsInitial, @AllowedRoles, @Conditions, @Configuration, @IsActive,
                    @TimeoutHours, @RequiresComments, @AllowDelegation);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflowStep,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowStep workflowStep)
        {
            const string sql = @"
                UPDATE WorkflowSteps
                SET StepCode = @StepCode,
                    StepName = @StepName,
                    StepType = @StepType,
                    StepOrder = @StepOrder,
                    NextStepId = @NextStepId,
                    IsFinal = @IsFinal,
                    IsInitial = @IsInitial,
                    AllowedRoles = @AllowedRoles,
                    Conditions = @Conditions,
                    Configuration = @Configuration,
                    IsActive = @IsActive,
                    TimeoutHours = @TimeoutHours,
                    RequiresComments = @RequiresComments,
                    AllowDelegation = @AllowDelegation
                WHERE StepId = @StepId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflowStep,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkflowStep>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT ws.* 
                FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowStep>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = @"ws.StepId IN (
                SELECT ws2.StepId FROM WorkflowSteps ws2
                INNER JOIN Workflows w ON ws2.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID)";

            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    ws.StepName LIKE @SearchTerm OR 
                    ws.StepCode LIKE @SearchTerm OR
                    ws.StepType LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowStep>(
                "WorkflowSteps ws",
                "ws.StepOrder, ws.StepName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = @"
                DELETE ws FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.StepId = @StepId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { StepId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                DELETE ws FROM WorkflowSteps ws
                INNER JOIN Workflows w ON ws.WorkflowId = w.WorkflowId
                WHERE ws.WorkflowId = @WorkflowId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
