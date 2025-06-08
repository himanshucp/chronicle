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
    public class WorkflowAssignmentRepository : DapperRepository<WorkflowAssignment, int>, IWorkflowAssignmentRepository
    {
        public WorkflowAssignmentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "WorkflowAssignments", "AssignmentId")
        {
        }

        public async Task<WorkflowAssignment> GetByIdAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignmentId = @AssignmentId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowAssignment>(
                sql,
                new { AssignmentId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowAssignment> GetActiveAssignmentAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.InstanceId = @InstanceId AND wa.IsActive = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowAssignment>(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.InstanceId = @InstanceId AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByAssignedToAsync(string assignedTo, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedTo = @AssignedTo AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedTo = assignedTo, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetActiveByAssignedToAsync(string assignedTo, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedTo = @AssignedTo AND wa.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedTo = assignedTo, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByAssignedRoleAsync(string assignedRole, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedRole = @AssignedRole AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedRole = assignedRole, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByStepIdAsync(int stepId, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.StepId = @StepId AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { StepId = stepId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByAssignedByAsync(string assignedBy, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedBy = @AssignedBy AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedBy = assignedBy, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedDate >= @StartDate AND wa.AssignedDate <= @EndDate 
                AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { StartDate = startDate, EndDate = endDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetActiveAssignmentsAsync(int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.IsActive = 1 AND w.TenantID = @TenantID
                ORDER BY wa.AssignedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetCompletedAssignmentsAsync(int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.IsActive = 0 AND wa.CompletedDate IS NOT NULL AND w.TenantID = @TenantID
                ORDER BY wa.CompletedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetOverdueAssignmentsAsync(int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.IsActive = 1 AND wi.DueDate < @CurrentDate 
                AND w.TenantID = @TenantID
                ORDER BY wi.DueDate";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { CurrentDate = DateTime.UtcNow, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<int> GetActiveAssignmentCountAsync(string assignedTo, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignedTo = @AssignedTo AND wa.IsActive = 1 AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { AssignedTo = assignedTo, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowAssignment workflowAssignment)
        {
            const string sql = @"
                INSERT INTO WorkflowAssignments (
                    InstanceId, StepId, AssignedTo, AssignedRole, AssignedDate,
                    CompletedDate, AssignedBy, Notes, IsActive)
                VALUES (
                    @InstanceId, @StepId, @AssignedTo, @AssignedRole, @AssignedDate,
                    @CompletedDate, @AssignedBy, @Notes, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set assignment date if not set
            if (workflowAssignment.AssignedDate == default(DateTime))
            {
                workflowAssignment.AssignedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflowAssignment,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowAssignment workflowAssignment)
        {
            const string sql = @"
                UPDATE WorkflowAssignments
                SET AssignedTo = @AssignedTo,
                    AssignedRole = @AssignedRole,
                    CompletedDate = @CompletedDate,
                    Notes = @Notes,
                    IsActive = @IsActive
                WHERE AssignmentId = @AssignmentId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                workflowAssignment,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT wa.* 
                FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowAssignment>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = @"wa.AssignmentId IN (
                SELECT wa2.AssignmentId FROM WorkflowAssignments wa2
                INNER JOIN WorkflowInstances wi ON wa2.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID)";

            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    wa.AssignedTo LIKE @SearchTerm OR 
                    wa.AssignedRole LIKE @SearchTerm OR
                    wa.AssignedBy LIKE @SearchTerm OR
                    wa.Notes LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowAssignment>(
                "WorkflowAssignments wa",
                "wa.AssignedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = @"
                DELETE wa FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.AssignmentId = @AssignmentId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { AssignmentId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByInstanceIdAsync(int instanceId, int tenantId)
        {
            const string sql = @"
                DELETE wa FROM WorkflowAssignments wa
                INNER JOIN WorkflowInstances wi ON wa.InstanceId = wi.InstanceId
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wa.InstanceId = @InstanceId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { InstanceId = instanceId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
