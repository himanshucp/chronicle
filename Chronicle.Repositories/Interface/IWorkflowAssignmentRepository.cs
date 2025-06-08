using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IWorkflowAssignmentRepository : IRepository<WorkflowAssignment, int>
    {
        Task<WorkflowAssignment> GetByIdAsync(int id, int tenantId);
        Task<WorkflowAssignment> GetActiveAssignmentAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByAssignedToAsync(string assignedTo, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetActiveByAssignedToAsync(string assignedTo, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByAssignedRoleAsync(string assignedRole, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByStepIdAsync(int stepId, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByAssignedByAsync(string assignedBy, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetActiveAssignmentsAsync(int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetCompletedAssignmentsAsync(int tenantId);
        Task<int> InsertAsync(WorkflowAssignment workflowAssignment);
        Task<bool> UpdateAsync(WorkflowAssignment workflowAssignment);
        Task<IEnumerable<WorkflowAssignment>> GetAllAsync(int tenantId);
        Task<PagedResult<WorkflowAssignment>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByInstanceIdAsync(int instanceId, int tenantId);
        Task<int> GetActiveAssignmentCountAsync(string assignedTo, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetOverdueAssignmentsAsync(int tenantId);
    }
}
