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
        Task<WorkflowAssignment> GetByIdAsync(int assignmentId);
        Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId);
        Task<IEnumerable<WorkflowAssignment>> GetByAssignedToAsync(string assignedTo);
        Task<IEnumerable<WorkflowAssignment>> GetActiveAssignmentsAsync(string assignedTo);
        Task<WorkflowAssignment> GetActiveAssignmentAsync(int instanceId, int stepId);
        Task<PagedResult<WorkflowAssignment>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
