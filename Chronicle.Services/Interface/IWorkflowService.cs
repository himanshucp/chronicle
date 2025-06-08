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
        Task<ServiceResult<Workflow>> GetWorkflowByIdAsync(int workflowId, int tenantId);
        Task<ServiceResult<Workflow>> GetByNameAsync(string workflowName, int tenantId);
        Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByModuleAsync(string module, int tenantId);
        Task<ServiceResult<IEnumerable<Workflow>>> GetActiveWorkflowsAsync(int tenantId);
        Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByCreatedByAsync(string createdBy, int tenantId);
        Task<ServiceResult<int>> CreateWorkflowAsync(Workflow workflow, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(Workflow workflow, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int workflowId, int tenantId);
        Task<IEnumerable<Workflow>> GetWorkflowsAsync(int tenantId);
        Task<ServiceResult<PagedResult<Workflow>>> GetPagedWorkflowsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<Workflow>> GetByNameAndModuleAsync(string workflowName, string module, int tenantId);
        Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByVersionAsync(int version, int tenantId);
        Task<ServiceResult<bool>> DeactivateWorkflowAsync(int workflowId, int tenantId);
        Task<ServiceResult<bool>> ActivateWorkflowAsync(int workflowId, int tenantId);
        Task<ServiceResult<Workflow>> CloneWorkflowAsync(int workflowId, string newWorkflowName, int tenantId, string createdBy);
    }
}
