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
            : base(unitOfWork, "WorkflowAssignment", "AssignmentId")
        {
        }

        public async Task<WorkflowAssignment> GetByIdAsync(int assignmentId)
        {
            const string sql = "SELECT * FROM WorkflowAssignment WHERE AssignmentId = @AssignmentId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowAssignment>(
                sql,
                new { AssignmentId = assignmentId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId)
        {
            const string sql = "SELECT * FROM WorkflowAssignment WHERE InstanceId = @InstanceId ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { InstanceId = instanceId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByAssignedToAsync(string assignedTo)
        {
            const string sql = "SELECT * FROM WorkflowAssignment WHERE AssignedTo = @AssignedTo ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedTo = assignedTo },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetActiveAssignmentsAsync(string assignedTo)
        {
            const string sql = "SELECT * FROM WorkflowAssignment WHERE AssignedTo = @AssignedTo AND IsActive = 1 ORDER BY AssignedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowAssignment>(
                sql,
                new { AssignedTo = assignedTo },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowAssignment> GetActiveAssignmentAsync(int instanceId, int stepId)
        {
            const string sql = "SELECT * FROM WorkflowAssignment WHERE InstanceId = @InstanceId AND StepId = @StepId AND IsActive = 1";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowAssignment>(
                sql,
                new { InstanceId = instanceId, StepId = stepId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowAssignment>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    AssignedTo LIKE @SearchTerm OR 
                    AssignedRole LIKE @SearchTerm OR
                    AssignedBy LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowAssignment>(
                "WorkflowAssignment",
                "AssignedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowAssignment assignment)
        {
            const string sql = @"
                INSERT INTO WorkflowAssignment (
                    InstanceId, StepId, AssignedTo, AssignedRole, AssignedDate, AssignedBy, Notes, IsActive)
                VALUES (
                    @InstanceId, @StepId, @AssignedTo, @AssignedRole, @AssignedDate, @AssignedBy, @Notes, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            if (assignment.AssignedDate == default)
            {
                assignment.AssignedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                assignment,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowAssignment assignment)
        {
            const string sql = @"
                UPDATE WorkflowAssignment
                SET CompletedDate = @CompletedDate,
                    Notes = @Notes,
                    IsActive = @IsActive
                WHERE AssignmentId = @AssignmentId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                assignment,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
