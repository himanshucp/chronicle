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
            : base(unitOfWork, "WorkflowInstances", "InstanceId")
        {
        }

        public async Task<WorkflowInstance> GetByIdAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.InstanceId = @InstanceId AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowInstance>(
                sql,
                new { InstanceId = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<WorkflowInstance> GetByIdWithHistoryAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT wi.*, wh.*
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                LEFT JOIN WorkflowHistory wh ON wi.InstanceId = wh.InstanceId
                WHERE wi.InstanceId = @InstanceId AND w.TenantID = @TenantID
                ORDER BY wh.TransitionDate DESC";

            var instanceDictionary = new Dictionary<int, WorkflowInstance>();

            var instances = await _unitOfWork.Connection.QueryAsync<WorkflowInstance, WorkflowHistory, WorkflowInstance>(
                sql,
                (instance, history) =>
                {
                    if (!instanceDictionary.TryGetValue(instance.InstanceId, out WorkflowInstance instanceEntry))
                    {
                        instanceEntry = instance;
                        instanceEntry.History = new List<WorkflowHistory>();
                        instanceDictionary.Add(instance.InstanceId, instanceEntry);
                    }

                    if (history != null)
                    {
                        instanceEntry.History.Add(history);
                    }

                    return instanceEntry;
                },
                new { InstanceId = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "HistoryId");

            return instances.FirstOrDefault();
        }

        public async Task<WorkflowInstance> GetByEntityAsync(int entityId, string entityType, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.EntityId = @EntityId AND wi.EntityType = @EntityType 
                AND wi.Status = @Status AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WorkflowInstance>(
                sql,
                new { EntityId = entityId, EntityType = entityType, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByWorkflowIdAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.WorkflowId = @WorkflowId AND w.TenantID = @TenantID
                ORDER BY wi.CreatedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { WorkflowId = workflowId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByCurrentStepAsync(int stepId, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.CurrentStepId = @StepId AND wi.Status = @Status AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { StepId = stepId, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(string status, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.Status = @Status AND w.TenantID = @TenantID
                ORDER BY wi.CreatedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Status = status, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetActiveInstancesAsync(int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.Status = @Status AND w.TenantID = @TenantID
                ORDER BY wi.Priority DESC, wi.CreatedDate";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByAssignedToAsync(string assignedTo, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.AssignedTo = @AssignedTo AND wi.Status = @Status AND w.TenantID = @TenantID
                ORDER BY wi.Priority DESC, wi.DueDate";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { AssignedTo = assignedTo, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetOverdueInstancesAsync(int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.DueDate < @CurrentDate AND wi.Status = @Status AND w.TenantID = @TenantID
                ORDER BY wi.DueDate, wi.Priority DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { CurrentDate = DateTime.UtcNow, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByPriorityAsync(int priority, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.Priority = @Priority AND wi.Status = @Status AND w.TenantID = @TenantID
                ORDER BY wi.CreatedDate";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Priority = priority, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByCreatedByAsync(string createdBy, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.CreatedBy = @CreatedBy AND w.TenantID = @TenantID
                ORDER BY wi.CreatedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { CreatedBy = createdBy, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.CreatedDate >= @StartDate AND wi.CreatedDate <= @EndDate AND w.TenantID = @TenantID
                ORDER BY wi.CreatedDate DESC";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { StartDate = startDate, EndDate = endDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<int> GetActiveInstanceCountAsync(int workflowId, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.WorkflowId = @WorkflowId AND wi.Status = @Status AND w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { WorkflowId = workflowId, Status = WorkflowStatusConstants.Active, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<WorkflowInstance>> GetStuckInstancesAsync(int daysThreshold, int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.Status = @Status 
                AND wi.LastTransitionDate < @ThresholdDate
                AND w.TenantID = @TenantID
                ORDER BY wi.LastTransitionDate";

            var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { Status = WorkflowStatusConstants.Active, ThresholdDate = thresholdDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(WorkflowInstance workflowInstance)
        {
            const string sql = @"
                INSERT INTO WorkflowInstances (
                    WorkflowId, EntityId, EntityType, CurrentStepId, Status,
                    CreatedDate, CompletedDate, LastTransitionDate, CreatedBy, CompletedBy,
                    Data, Variables, Priority, Notes, AssignedTo, DueDate)
                VALUES (
                    @WorkflowId, @EntityId, @EntityType, @CurrentStepId, @Status,
                    @CreatedDate, @CompletedDate, @LastTransitionDate, @CreatedBy, @CompletedBy,
                    @Data, @Variables, @Priority, @Notes, @AssignedTo, @DueDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (workflowInstance.CreatedDate == default(DateTime))
            {
                workflowInstance.CreatedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                workflowInstance,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(WorkflowInstance workflowInstance)
        {
            const string sql = @"
                UPDATE WorkflowInstances
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
                workflowInstance,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkflowInstance>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT wi.* 
                FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<WorkflowInstance>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<WorkflowInstance>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = @"wi.InstanceId IN (
                SELECT wi2.InstanceId FROM WorkflowInstances wi2
                INNER JOIN Workflows w ON wi2.WorkflowId = w.WorkflowId
                WHERE w.TenantID = @TenantID)";

            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    wi.EntityType LIKE @SearchTerm OR 
                    wi.CreatedBy LIKE @SearchTerm OR
                    wi.AssignedTo LIKE @SearchTerm OR
                    wi.Notes LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<WorkflowInstance>(
                "WorkflowInstances wi",
                "wi.Priority DESC, wi.CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = @"
                DELETE wi FROM WorkflowInstances wi
                INNER JOIN Workflows w ON wi.WorkflowId = w.WorkflowId
                WHERE wi.InstanceId = @InstanceId AND w.TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { InstanceId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
