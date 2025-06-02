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
            : base(unitOfWork, "WorkflowTransition", "TransitionId")
        {
        }

        public async Task<WorkflowTransition> GetByIdAsync(int transitionId)
        {
            const string sql = "SELECT * FROM WorkflowTransition WHERE TransitionId = @TransitionId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowTransition>(
                sql,
                new { TransitionId = transitionId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByWorkflowIdAsync(int workflowId)
        {
            const string sql = "SELECT * FROM WorkflowTransition WHERE WorkflowId = @WorkflowId AND IsActive = 1 ORDER BY Priority DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByFromStepIdAsync(int fromStepId)
        {
            const string sql = "SELECT * FROM WorkflowTransition WHERE FromStepId = @FromStepId AND IsActive = 1 ORDER BY Priority DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { FromStepId = fromStepId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowTransition>> GetByToStepIdAsync(int toStepId)
        {
            const string sql = "SELECT * FROM WorkflowTransition WHERE ToStepId = @ToStepId AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<WorkflowTransition>(
                sql,
                new { ToStepId = toStepId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowTransition> GetByActionCodeAsync(int workflowId, string actionCode)
        {
            const string sql = "SELECT * FROM WorkflowTransition WHERE WorkflowId = @WorkflowId AND ActionCode = @ActionCode AND IsActive = 1";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowTransition>(
                sql,
                new { WorkflowId = workflowId, ActionCode = actionCode },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowTransition>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    ActionCode LIKE @SearchTerm OR 
                    ActionName LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowTransition>(
                "WorkflowTransition",
                "Priority DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowTransition transition)
        {
            const string sql = @"
                INSERT INTO WorkflowTransition (
                    WorkflowId, FromStepId, ToStepId, ActionCode, ActionName, Condition, AllowedRoles,
                    IsActive, Priority, Configuration, RequiresApproval, RequiresComments, NotificationRoles)
                VALUES (
                    @WorkflowId, @FromStepId, @ToStepId, @ActionCode, @ActionName, @Condition, @AllowedRoles,
                    @IsActive, @Priority, @Configuration, @RequiresApproval, @RequiresComments, @NotificationRoles);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                transition,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowTransition transition)
        {
            const string sql = @"
                UPDATE WorkflowTransition
                SET ActionCode = @ActionCode,
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
                transition,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
