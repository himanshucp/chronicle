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
        Task<WorkflowTransition> GetByIdAsync(int transitionId);
        Task<IEnumerable<WorkflowTransition>> GetByWorkflowIdAsync(int workflowId);
        Task<IEnumerable<WorkflowTransition>> GetByFromStepIdAsync(int fromStepId);
        Task<IEnumerable<WorkflowTransition>> GetByToStepIdAsync(int toStepId);
        Task<WorkflowTransition> GetByActionCodeAsync(int workflowId, string actionCode);
        Task<PagedResult<WorkflowTransition>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
