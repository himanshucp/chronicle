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
        Task<ServiceResult<WorkflowStep>> GetWorkflowStepByIdAsync(int stepId, int tenantId);
        Task<ServiceResult<WorkflowStep>> GetWorkflowStepWithTransitionsAsync(int stepId, int tenantId);
        Task<ServiceResult<WorkflowStep>> GetByStepCodeAsync(string stepCode, int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByWorkflowIdAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByTypeAsync(string stepType, int tenantId);
        Task<ServiceResult<WorkflowStep>> GetInitialStepAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetFinalStepsAsync(int workflowId, int tenantId);
        Task<ServiceResult<WorkflowStep>> GetNextStepAsync(int currentStepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetActiveStepsByWorkflowAsync(int workflowId, int tenantId);
        Task<ServiceResult<int>> CreateWorkflowStepAsync(WorkflowStep workflowStep, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowStep workflowStep, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int stepId, int tenantId);
        Task<ServiceResult<bool>> DeleteByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetWorkflowStepsAsync(int tenantId);
        Task<ServiceResult<PagedResult<WorkflowStep>>> GetPagedWorkflowStepsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByRoleAsync(string role, int workflowId, int tenantId);
        Task<ServiceResult<bool>> ReorderStepsAsync(int workflowId, List<int> stepIds, int tenantId);
        Task<ServiceResult<bool>> ActivateStepAsync(int stepId, int tenantId);
        Task<ServiceResult<bool>> DeactivateStepAsync(int stepId, int tenantId);
        Task<ServiceResult<WorkflowStep>> CloneStepAsync(int stepId, int targetWorkflowId, int tenantId);
        Task<ServiceResult<bool>> ValidateWorkflowStepsAsync(int workflowId, int tenantId);
    }
}
