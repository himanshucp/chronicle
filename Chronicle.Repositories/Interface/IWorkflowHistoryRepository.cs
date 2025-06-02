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
        Task<WorkflowHistory> GetByIdAsync(int historyId);
        Task<IEnumerable<WorkflowHistory>> GetByInstanceIdAsync(int instanceId);
        Task<IEnumerable<WorkflowHistory>> GetByActionByAsync(string actionBy);
        Task<PagedResult<WorkflowHistory>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
