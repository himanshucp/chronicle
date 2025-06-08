using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.Interface
{
    public interface IWorkflowHistoryService
    {
        Task<ServiceResult<WorkflowHistory>> GetWorkflowHistoryByIdAsync(int historyId, int tenantId);
        Task<ServiceResult<WorkflowHistory>> GetHistoryWithStepsAsync(int historyId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByInstanceIdAsync(int instanceId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByActionByAsync(string actionBy, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByActionCodeAsync(string actionCode, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByFromStepAsync(int fromStepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByToStepAsync(int toStepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByWorkflowIdAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByAssignedToAsync(string assignedTo, int tenantId);
        Task<ServiceResult<int>> CreateHistoryEntryAsync(WorkflowHistory workflowHistory, int tenantId);
        Task<ServiceResult<bool>> UpdateHistoryAsync(WorkflowHistory workflowHistory, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int historyId, int tenantId);
        Task<ServiceResult<bool>> DeleteByInstanceIdAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowHistory>> GetWorkflowHistoryAsync(int tenantId);
        Task<ServiceResult<PagedResult<WorkflowHistory>>> GetPagedWorkflowHistoryAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<WorkflowHistory>> GetLastTransitionAsync(int instanceId, int tenantId);
        Task<ServiceResult<double>> GetAverageStepDurationAsync(int fromStepId, int toStepId, int tenantId);
        Task<ServiceResult<int>> GetTransitionCountAsync(int fromStepId, int toStepId, int tenantId);
        Task<ServiceResult<WorkflowHistory>> CreateTransitionHistoryAsync(int instanceId, int? fromStepId, int? toStepId, int? transitionId, string action, string actionBy, int tenantId, string comments = null, string actionByRole = null);
        Task<ServiceResult<bool>> UpdateDurationAsync(int historyId, double durationMinutes, int tenantId);
        Task<ServiceResult<Dictionary<string, object>>> GetWorkflowMetricsAsync(int workflowId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetUserActivityAsync(string userId, DateTime? startDate, DateTime? endDate, int tenantId);
    }
}
