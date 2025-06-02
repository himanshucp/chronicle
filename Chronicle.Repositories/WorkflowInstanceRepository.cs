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
    public class WorkflowInstanceRepository : DapperRepository<WorkflowInstance, int>, IWorkflowInstanceRepository
    {
        public WorkflowInstanceRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "WorkflowInstance", "InstanceId")
        {
        }

        public async Task<WorkflowInstance> GetByIdAsync(int instanceId)
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE InstanceId = @InstanceId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowInstance>(
                sql,
                new { InstanceId = instanceId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByWorkflowIdAsync(int workflowId)
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE WorkflowId = @WorkflowId ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { WorkflowId = workflowId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(string entityType, int entityId)
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE EntityType = @EntityType AND EntityId = @EntityId ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { EntityType = entityType, EntityId = entityId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(string status)
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE Status = @Status ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Status = status },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByAssignedToAsync(string assignedTo)
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE AssignedTo = @AssignedTo ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { AssignedTo = assignedTo },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetActiveInstancesAsync()
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE Status = @Status ORDER BY CreatedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Status = WorkflowStatusConstants.Active },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetOverdueInstancesAsync()
        {
            const string sql = "SELECT * FROM WorkflowInstance WHERE DueDate < @Now AND Status = @Status";
            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Now = DateTime.UtcNow, Status = WorkflowStatusConstants.Active },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowInstance>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = null;
            object parameters = null;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    EntityType LIKE @SearchTerm OR 
                    Status LIKE @SearchTerm OR
                    AssignedTo LIKE @SearchTerm OR
                    CreatedBy LIKE @SearchTerm";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowInstance>(
                "WorkflowInstance",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowInstance instance)
        {
            const string sql = @"
                INSERT INTO WorkflowInstance (
                    WorkflowId, EntityId, EntityType, CurrentStepId, Status, CreatedDate, CreatedBy,
                    Data, Variables, Priority, Notes, AssignedTo, DueDate)
                VALUES (
                    @WorkflowId, @EntityId, @EntityType, @CurrentStepId, @Status, @CreatedDate, @CreatedBy,
                    @Data, @Variables, @Priority, @Notes, @AssignedTo, @DueDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            if (instance.CreatedDate == default)
            {
                instance.CreatedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                instance,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowInstance instance)
        {
            const string sql = @"
                UPDATE WorkflowInstance
                SET CurrentStepId = @CurrentStepId,
                    Status = @Status,
                    CompletedDate = @CompletedDate,
                    LastTransitionDate = @LastTransitionDate,
                    CompletedBy = @CompletedBy,
                    Data = @Data,
                    Variables = @Variables,
                    Priority = @Priority,
                    Notes = @Notes,
                    AssignedTo = @AssignedTo,
                    DueDate = @DueDate
                WHERE InstanceId = @InstanceId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                instance,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
