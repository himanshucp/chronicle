using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowHistoryRepository : IRepository<WorkflowHistory, int>
    {
        Task<WorkflowHistory> GetByIdAsync(int id, int tenantId);
        Task<WorkflowHistory> GetByIdWithStepsAsync(int id, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByInstanceIdAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByActionByAsync(string actionBy, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByActionCodeAsync(string actionCode, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByFromStepAsync(int fromStepId, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByToStepAsync(int toStepId, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByWorkflowIdAsync(int workflowId, int tenantId);
        Task<int> InsertAsync(WorkflowHistory workflowHistory);
        Task<bool> UpdateAsync(WorkflowHistory workflowHistory);
        Task<IEnumerable<WorkflowHistory>> GetAllAsync(int tenantId);
        Task<PagedResult<WorkflowHistory>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByInstanceIdAsync(int instanceId, int tenantId);
        Task<WorkflowHistory> GetLastTransitionAsync(int instanceId, int tenantId);
        Task<double> GetAverageStepDurationAsync(int fromStepId, int toStepId, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetByAssignedToAsync(string assignedTo, int tenantId);
        Task<int> GetTransitionCountAsync(int fromStepId, int toStepId, int tenantId);
    }
}
