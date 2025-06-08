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
        Task<WorkflowStep> GetByIdAsync(int id, int tenantId);
        Task<WorkflowStep> GetByIdWithTransitionsAsync(int id, int tenantId);
        Task<WorkflowStep> GetByStepCodeAsync(string stepCode, int workflowId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetByStepTypeAsync(string stepType, int tenantId);
        Task<WorkflowStep> GetInitialStepAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetFinalStepsAsync(int workflowId, int tenantId);
        Task<WorkflowStep> GetNextStepAsync(int currentStepId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetActiveStepsByWorkflowAsync(int workflowId, int tenantId);
        Task<int> InsertAsync(WorkflowStep workflowStep);
        Task<bool> UpdateAsync(WorkflowStep workflowStep);
        Task<IEnumerable<WorkflowStep>> GetAllAsync(int tenantId);
        Task<PagedResult<WorkflowStep>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByWorkflowIdAsync(int workflowId, int tenantId);
        Task<IEnumerable<WorkflowStep>> GetStepsByRoleAsync(string role, int workflowId, int tenantId);
        Task<int> GetMaxStepOrderAsync(int workflowId, int tenantId);
    }
}
