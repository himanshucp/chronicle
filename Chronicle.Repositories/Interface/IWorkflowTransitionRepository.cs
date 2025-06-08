using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowTransitionRepository : IRepository<WorkflowTransition, int>
    {
        Task<WorkflowTransition> GetByIdAsync(int id, int tenantId);
        Task<WorkflowTransition> GetByIdWithStepsAsync(int id, int tenantId);
        Task<WorkflowTransition> GetByActionCodeAsync(string actionCode, int fromStepId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetByFromStepIdAsync(int fromStepId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetByToStepIdAsync(int toStepId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetActiveTransitionsByWorkflowAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetTransitionsByRoleAsync(string role, int workflowId, int tenantId);
        Task<int> InsertAsync(WorkflowTransition workflowTransition);
        Task<bool> UpdateAsync(WorkflowTransition workflowTransition);
        Task<IEnumerable<WorkflowTransition>> GetAllAsync(int tenantId);
        Task<PagedResult<WorkflowTransition>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowTransition>> GetTransitionsRequiringApprovalAsync(int workflowId, int tenantId);
        Task<WorkflowTransition> GetHighestPriorityTransitionAsync(int fromStepId, int tenantId);
        Task<bool> TransitionExistsAsync(int fromStepId, int toStepId, string actionCode, int tenantId);
    }
}
