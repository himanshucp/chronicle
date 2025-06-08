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
            : base(unitOfWork, "Workflows", "WorkflowId")
        {
        }

        public async Task<Workflow> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE WorkflowId = @WorkflowId AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Workflow>(
                sql,
                new { WorkflowId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Workflow> GetByNameAsync(string workflowName, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE WorkflowName = @WorkflowName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Workflow>(
                sql,
                new { WorkflowName = workflowName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetByModuleAsync(string module, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE Module = @Module AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { Module = module, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetByCreatedByAsync(string createdBy, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE CreatedBy = @CreatedBy AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { CreatedBy = createdBy, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Workflow> GetByNameAndModuleAsync(string workflowName, string module, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE WorkflowName = @WorkflowName AND Module = @Module AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Workflow>(
                sql,
                new { WorkflowName = workflowName, Module = module, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Workflow>> GetByVersionAsync(int version, int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE Version = @Version AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { Version = version, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Workflow workflow)
        {
            const string sql = @"
                INSERT INTO Workflows (
                    TenantID, WorkflowName, Description, Module, CreatedDate, 
                    IsActive, CreatedBy, Configuration, Version, LastModified, LastModifiedBy)
                VALUES (
                    @TenantID, @WorkflowName, @Description, @Module, @CreatedDate, 
                    @IsActive, @CreatedBy, @Configuration, @Version, @LastModified, @LastModifiedBy);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (workflow.CreatedDate == default(DateTime))
            {
                workflow.CreatedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflow,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Workflow workflow)
        {
            const string sql = @"
                UPDATE Workflows
                SET WorkflowName = @WorkflowName,
                    Description = @Description,
                    Module = @Module,
                    IsActive = @IsActive,
                    Configuration = @Configuration,
                    Version = @Version,
                    LastModified = @LastModified,
                    LastModifiedBy = @LastModifiedBy
                WHERE WorkflowId = @WorkflowId AND TenantID = @TenantID";

            // Set modification date
            workflow.LastModified = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflow,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Workflow>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Workflows WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Workflow>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Workflow>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    WorkflowName LIKE @SearchTerm OR 
                    Module LIKE @SearchTerm OR
                    Description LIKE @SearchTerm OR
                    CreatedBy LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Workflow>(
                "Workflows",
                "WorkflowName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM Workflows WHERE WorkflowId = @WorkflowId AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { WorkflowId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
