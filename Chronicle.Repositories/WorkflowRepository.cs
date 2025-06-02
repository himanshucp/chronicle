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
    public class WorkflowRepository : DapperRepository<Workflow, int>, IWorkflowRepository
    {
        public WorkflowRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Workflow", "WorkflowId")
        {
        }

        public async Task<Workflow> GetByNameAsync(string workflowName)
        {
            const string sql = "SELECT * FROM Workflow WHERE WorkflowName = @WorkflowName";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Workflow>(
                sql,
                new { WorkflowName = workflowName },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetByModuleAsync(string module)
        {
            const string sql = "SELECT * FROM Workflow WHERE Module = @Module AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { Module = module },
                _unitOfWork.Transaction);
        }

        public async Task<Workflow> GetByIdAsync(int workflowId)
        {
            const string sql = "SELECT * FROM Workflow WHERE WorkflowId = @WorkflowId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Workflow>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync()
        {
            const string sql = "SELECT * FROM Workflow WHERE IsActive = 1 ORDER BY WorkflowName";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                null,
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Workflow>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    WorkflowName LIKE @SearchTerm OR 
                    Module LIKE @SearchTerm OR
                    Description LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Workflow>(
                "Workflow",
                "WorkflowName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Workflow workflow)
        {
            const string sql = @"
                INSERT INTO Workflow (
                    WorkflowName, Description, Module, CreatedDate, IsActive, CreatedBy, 
                    Configuration, Version, LastModified, LastModifiedBy)
                VALUES (
                    @WorkflowName, @Description, @Module, @CreatedDate, @IsActive, @CreatedBy, 
                    @Configuration, @Version, @LastModified, @LastModifiedBy);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            if (workflow.CreatedDate == default)
            {
                workflow.CreatedDate = DateTime.UtcNow;
                workflow.LastModified = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflow,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Workflow workflow)
        {
            const string sql = @"
                UPDATE Workflow
                SET WorkflowName = @WorkflowName,
                    Description = @Description,
                    Module = @Module,
                    IsActive = @IsActive,
                    Configuration = @Configuration,
                    Version = @Version,
                    LastModified = @LastModified,
                    LastModifiedBy = @LastModifiedBy
                WHERE WorkflowId = @WorkflowId";

            workflow.LastModified = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflow,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
