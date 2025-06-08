using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.Interface
{
    public interface IWorkflowTransitionService
    {
        Task<ServiceResult<WorkflowTransition>> GetWorkflowTransitionByIdAsync(int transitionId, int tenantId);
        Task<ServiceResult<WorkflowTransition>> GetTransitionWithStepsAsync(int transitionId, int tenantId);
        Task<ServiceResult<WorkflowTransition>> GetByActionCodeAsync(string actionCode, int fromStepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsByWorkflowIdAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetAvailableTransitionsAsync(int fromStepId, int tenantId, string userRole = null);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetIncomingTransitionsAsync(int toStepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetActiveTransitionsByWorkflowAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsByRoleAsync(string role, int workflowId, int tenantId);
        Task<ServiceResult<int>> CreateWorkflowTransitionAsync(WorkflowTransition workflowTransition, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowTransition workflowTransition, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int transitionId, int tenantId);
        Task<ServiceResult<bool>> DeleteByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetWorkflowTransitionsAsync(int tenantId);
        Task<ServiceResult<PagedResult<WorkflowTransition>>> GetPagedWorkflowTransitionsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsRequiringApprovalAsync(int workflowId, int tenantId);
        Task<ServiceResult<WorkflowTransition>> GetHighestPriorityTransitionAsync(int fromStepId, int tenantId);
        Task<ServiceResult<bool>> ActivateTransitionAsync(int transitionId, int tenantId);
        Task<ServiceResult<bool>> DeactivateTransitionAsync(int transitionId, int tenantId);
        Task<ServiceResult<bool>> UpdateTransitionPriorityAsync(int transitionId, int newPriority, int tenantId);
        Task<ServiceResult<bool>> ValidateTransitionAsync(WorkflowTransition transition, int tenantId);
    }
}
