using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowRepository : IRepository<Workflow, int>
    {
        Task<Workflow> GetByNameAsync(string workflowName);
        Task<IEnumerable<Workflow>> GetByModuleAsync(string module);
        Task<Workflow> GetByIdAsync(int workflowId);
        Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync();
        Task<PagedResult<Workflow>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
