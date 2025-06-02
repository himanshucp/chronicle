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
        Task<WorkflowInstance> GetByIdAsync(int instanceId);
        Task<IEnumerable<WorkflowInstance>> GetByWorkflowIdAsync(int workflowId);
        Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(string status);
        Task<IEnumerable<WorkflowInstance>> GetByAssignedToAsync(string assignedTo);
        Task<IEnumerable<WorkflowInstance>> GetActiveInstancesAsync();
        Task<IEnumerable<WorkflowInstance>> GetOverdueInstancesAsync();
        Task<PagedResult<WorkflowInstance>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
