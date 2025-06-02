using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowStepRepository : IRepository<WorkflowStep, int>
    {
        Task<WorkflowStep> GetByIdAsync(int stepId);
        Task<IEnumerable<WorkflowStep>> GetByWorkflowIdAsync(int workflowId);
        Task<WorkflowStep> GetInitialStepAsync(int workflowId);
        Task<IEnumerable<WorkflowStep>> GetFinalStepsAsync(int workflowId);
        Task<WorkflowStep> GetByStepCodeAsync(int workflowId, string stepCode);
        Task<PagedResult<WorkflowStep>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }


}
