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
        Task<ServiceResult<WorkflowAssignment>> GetWorkflowAssignmentByIdAsync(int assignmentId, int tenantId);
        Task<ServiceResult<WorkflowAssignment>> GetActiveAssignmentAsync(int instanceId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByInstanceIdAsync(int instanceId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByAssignedToAsync(string assignedTo, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsByAssignedToAsync(string assignedTo, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByRoleAsync(string assignedRole, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByStepAsync(int stepId, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByAssignedByAsync(string assignedBy, int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsAsync(int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetCompletedAssignmentsAsync(int tenantId);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetOverdueAssignmentsAsync(int tenantId);
        Task<ServiceResult<WorkflowAssignment>> CreateAssignmentAsync(int instanceId, int stepId, string assignedTo, string assignedRole, string assignedBy, int tenantId, string notes = null);
        Task<ServiceResult<bool>> UpdateAssignmentAsync(WorkflowAssignment workflowAssignment, int tenantId);
        Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, int tenantId, string notes = null);
        Task<ServiceResult<bool>> ReassignAsync(int assignmentId, string newAssignedTo, string reassignedBy, int tenantId, string notes = null);
        Task<ServiceResult<bool>> DeleteAsync(int assignmentId, int tenantId);
        Task<ServiceResult<bool>> DeleteByInstanceIdAsync(int instanceId, int tenantId);
        Task<IEnumerable<WorkflowAssignment>> GetWorkflowAssignmentsAsync(int tenantId);
        Task<ServiceResult<PagedResult<WorkflowAssignment>>> GetPagedWorkflowAssignmentsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<ServiceResult<int>> GetActiveAssignmentCountAsync(string assignedTo, int tenantId);
        Task<ServiceResult<bool>> BulkAssignAsync(List<int> instanceIds, string assignedTo, string assignedRole, string assignedBy, int tenantId);
        Task<ServiceResult<Dictionary<string, object>>> GetAssignmentStatisticsAsync(string assignedTo, int tenantId);
        Task<ServiceResult<bool>> DeactivateOldAssignmentsAsync(int instanceId, int tenantId);
    }
}
