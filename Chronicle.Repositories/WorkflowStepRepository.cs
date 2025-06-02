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
            : base(unitOfWork, "WorkflowStep", "StepId")
        {
        }

        public async Task<WorkflowStep> GetByIdAsync(int stepId)
        {
            const string sql = "SELECT * FROM WorkflowStep WHERE StepId = @StepId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { StepId = stepId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetByWorkflowIdAsync(int workflowId)
        {
            const string sql = "SELECT * FROM WorkflowStep WHERE WorkflowId = @WorkflowId AND IsActive = 1 ORDER BY StepOrder";
            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowStep> GetInitialStepAsync(int workflowId)
        {
            const string sql = "SELECT * FROM WorkflowStep WHERE WorkflowId = @WorkflowId AND IsInitial = 1 AND IsActive = 1";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowStep>> GetFinalStepsAsync(int workflowId)
        {
            const string sql = "SELECT * FROM WorkflowStep WHERE WorkflowId = @WorkflowId AND IsFinal = 1 AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowStep> GetByStepCodeAsync(int workflowId, string stepCode)
        {
            const string sql = "SELECT * FROM WorkflowStep WHERE WorkflowId = @WorkflowId AND StepCode = @StepCode AND IsActive = 1";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowStep>(
                sql,
                new { WorkflowId = workflowId, StepCode = stepCode },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowStep>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    StepCode LIKE @SearchTerm OR 
                    StepName LIKE @SearchTerm OR
                    StepType LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowStep>(
                "WorkflowStep",
                "StepOrder",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowStep step)
        {
            const string sql = @"
                INSERT INTO WorkflowStep (
                    WorkflowId, StepCode, StepName, StepType, StepOrder, NextStepId, IsFinal, IsInitial,
                    AllowedRoles, Conditions, Configuration, IsActive, TimeoutHours, RequiresComments, AllowDelegation)
                VALUES (
                    @WorkflowId, @StepCode, @StepName, @StepType, @StepOrder, @NextStepId, @IsFinal, @IsInitial,
                    @AllowedRoles, @Conditions, @Configuration, @IsActive, @TimeoutHours, @RequiresComments, @AllowDelegation);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                step,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowStep step)
        {
            const string sql = @"
                UPDATE WorkflowStep
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
                step,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
