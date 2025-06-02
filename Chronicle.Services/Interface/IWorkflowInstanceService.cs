using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowInstanceService
    {
        Task<ServiceResult<WorkflowInstance>> GetInstanceByIdAsync(int instanceId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByWorkflowIdAsync(int workflowId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByEntityAsync(string entityType, int entityId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByStatusAsync(string status);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByAssignedToAsync(string assignedTo);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetActiveInstancesAsync();
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetOverdueInstancesAsync();
        Task<ServiceResult<PagedResult<WorkflowInstance>>> GetInstancesAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> StartWorkflowAsync(int workflowId, int entityId, string entityType, string startedBy);
        Task<ServiceResult<bool>> TransitionAsync(int instanceId, string actionCode, string actionBy, string comments = null);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowInstance instance);
        Task<ServiceResult<bool>> DeleteAsync(int instanceId, int tenantId);
    }
}
