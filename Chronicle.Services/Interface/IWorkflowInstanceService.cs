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
        Task<ServiceResult<WorkflowInstance>> GetWorkflowInstanceByIdAsync(int instanceId, int tenantId);
        Task<ServiceResult<WorkflowInstance>> GetInstanceWithHistoryAsync(int instanceId, int tenantId);
        Task<ServiceResult<WorkflowInstance>> GetByEntityAsync(int entityId, string entityType, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByWorkflowIdAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByCurrentStepAsync(int stepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByStatusAsync(string status, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetActiveInstancesAsync(int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByAssignedToAsync(string assignedTo, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetOverdueInstancesAsync(int tenantId);
        Task<ServiceResult<WorkflowInstance>> StartWorkflowAsync(int workflowId, int entityId, string entityType, string createdBy, int tenantId, string data = null, string variables = null);
        Task<ServiceResult<bool>> TransitionWorkflowAsync(int instanceId, int transitionId, string performedBy, int tenantId, string comments = null);
        Task<ServiceResult<bool>> UpdateInstanceAsync(WorkflowInstance workflowInstance, int tenantId);
        Task<ServiceResult<bool>> CompleteWorkflowAsync(int instanceId, string completedBy, int tenantId);
        Task<ServiceResult<bool>> CancelWorkflowAsync(int instanceId, string cancelledBy, int tenantId, string reason = null);
        Task<ServiceResult<bool>> AssignWorkflowAsync(int instanceId, string assignedTo, int tenantId);
        Task<ServiceResult<bool>> UpdatePriorityAsync(int instanceId, int priority, int tenantId);
        Task<ServiceResult<bool>> UpdateDueDateAsync(int instanceId, DateTime? dueDate, int tenantId);
        Task<ServiceResult<bool>> UpdateVariablesAsync(int instanceId, string variables, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(int tenantId);
        Task<ServiceResult<PagedResult<WorkflowInstance>>> GetPagedWorkflowInstancesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<ServiceResult<int>> GetActiveInstanceCountAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetStuckInstancesAsync(int daysThreshold, int tenantId);
        Task<ServiceResult<bool>> RestartWorkflowAsync(int instanceId, string restartedBy, int tenantId);
    }
}
