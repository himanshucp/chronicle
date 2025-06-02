using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowStepService
    {
        Task<ServiceResult<WorkflowStep>> GetStepByIdAsync(int stepId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetByWorkflowIdAsync(int workflowId);
        Task<ServiceResult<WorkflowStep>> GetInitialStepAsync(int workflowId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetFinalStepsAsync(int workflowId);
        Task<ServiceResult<WorkflowStep>> GetByStepCodeAsync(int workflowId, string stepCode);
        Task<ServiceResult<PagedResult<WorkflowStep>>> GetStepsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateStepAsync(WorkflowStep step);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowStep step);
        Task<ServiceResult<bool>> DeleteAsync(int stepId, int tenantId);
    }
}
