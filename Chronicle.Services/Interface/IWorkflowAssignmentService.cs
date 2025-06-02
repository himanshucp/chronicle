using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IWorkflowAssignmentService
    {
        Task<ServiceResult<WorkflowAssignment>> GetAssignmentByIdAsync(int assignmentId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetByInstanceIdAsync(int instanceId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetByAssignedToAsync(string assignedTo);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsAsync(string assignedTo);
        Task<ServiceResult<WorkflowAssignment>> GetActiveAssignmentAsync(int instanceId, int stepId);
        Task<ServiceResult<PagedResult<WorkflowAssignment>>> GetAssignmentsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateAssignmentAsync(WorkflowAssignment assignment);
        Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, string completedBy);
        Task<ServiceResult<bool>> UpdateAsync(WorkflowAssignment assignment);
        Task<ServiceResult<bool>> DeleteAsync(int assignmentId,int tenantId);
    }
}
