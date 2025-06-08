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
        Task<Workflow> GetByIdAsync(int id, int tenantId);
        Task<Workflow> GetByNameAsync(string workflowName, int tenantId);
        Task<IEnumerable<Workflow>> GetByModuleAsync(string module, int tenantId);
        Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync(int tenantId);
        Task<IEnumerable<Workflow>> GetByCreatedByAsync(string createdBy, int tenantId);
        Task<int> InsertAsync(Workflow workflow);
        Task<bool> UpdateAsync(Workflow workflow);
        Task<IEnumerable<Workflow>> GetAllAsync(int tenantId);
        Task<PagedResult<Workflow>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<Workflow> GetByNameAndModuleAsync(string workflowName, string module, int tenantId);
        Task<IEnumerable<Workflow>> GetByVersionAsync(int version, int tenantId);
    }
}
