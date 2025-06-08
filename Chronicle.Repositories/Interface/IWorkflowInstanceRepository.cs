using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance, int>
    {
        Task<WorkflowInstance> GetByIdAsync(int id, int tenantId);
        Task<WorkflowInstance> GetByIdWithHistoryAsync(int id, int tenantId);
        Task<WorkflowInstance> GetByEntityAsync(int entityId, string entityType, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByCurrentStepAsync(int stepId, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(string status, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetActiveInstancesAsync(int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByAssignedToAsync(string assignedTo, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetOverdueInstancesAsync(int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByPriorityAsync(int priority, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByCreatedByAsync(string createdBy, int tenantId);
        Task<int> InsertAsync(WorkflowInstance workflowInstance);
        Task<bool> UpdateAsync(WorkflowInstance workflowInstance);
        Task<IEnumerable<WorkflowInstance>> GetAllAsync(int tenantId);
        Task<PagedResult<WorkflowInstance>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<int> GetActiveInstanceCountAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetStuckInstancesAsync(int daysThreshold, int tenantId);
    }
}
