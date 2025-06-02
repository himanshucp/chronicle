using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowTransitionService
    {
        Task<ServiceResult<WorkflowTransition>> GetTransitionByIdAsync(int transitionId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByWorkflowIdAsync(int workflowId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByFromStepIdAsync(int fromStepId);
        Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByToStepIdAsync(int toStepId);
        Task<ServiceResult<WorkflowTransition>> GetByActionCodeAsync(int workflowId, string actionCode);
        Task<ServiceResult<PagedResult<WorkflowTransition>>> GetTransitionsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateTransitionAsync(WorkflowTransition transition);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowTransition transition);
        Task<ServiceResult<bool>> DeleteAsync(int transitionId, int tenantId);
    }

}
