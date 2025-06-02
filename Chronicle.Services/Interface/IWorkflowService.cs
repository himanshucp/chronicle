using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowService
    {
        Task<ServiceResult<Workflow>> GetWorkflowByIdAsync(int workflowId);
        Task<ServiceResult<Workflow>> GetByNameAsync(string workflowName);
        Task<ServiceResult<IEnumerable<Workflow>>> GetByModuleAsync(string module);
        Task<ServiceResult<IEnumerable<Workflow>>> GetActiveWorkflowsAsync();
        Task<ServiceResult<PagedResult<Workflow>>> GetWorkflowsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateWorkflowAsync(Workflow workflow);
        Task<ServiceResult<bool>> UpdateAsync(Workflow workflow);
        Task<ServiceResult<bool>> DeleteAsync(int workflowId, int tenantId);
    }
}
