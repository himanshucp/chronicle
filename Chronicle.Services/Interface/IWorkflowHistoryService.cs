using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowHistoryService
    {
        Task<ServiceResult<WorkflowHistory>> GetHistoryByIdAsync(int historyId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetByInstanceIdAsync(int instanceId);
        Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetByActionByAsync(string actionBy);
        Task<ServiceResult<PagedResult<WorkflowHistory>>> GetHistoryAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateHistoryAsync(WorkflowHistory history);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowHistory history);
        Task<ServiceResult<bool>> DeleteAsync(int historyId,int tenantId);
    }
}
