using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.Interface
{
    public interface IInspectorAssignmentService
    {
        Task<ServiceResult<InspectorAssignment>> GetInspectorAssignmentByIdAsync(int assignmentId);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByInspectorUserIdAsync(int inspectorUserId);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignedByAsync(int assignedBy);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignmentTypeAsync(string assignmentType);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByStatusAsync(string status);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetPendingAssignmentsAsync();
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetAcceptedAssignmentsAsync();
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetDeclinedAssignmentsAsync();
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetCompletedAssignmentsAsync();
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetOverdueAssignmentsAsync();
        Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetActiveAssignmentsForInspectorAsync(int inspectorUserId);
        Task<ServiceResult<InspectorAssignment>> GetActiveAssignmentForInspectionRequestAsync(int inspectionRequestId);
        Task<IEnumerable<InspectorAssignment>> GetInspectorAssignmentsAsync();
        Task<ServiceResult<PagedResult<InspectorAssignment>>> GetPagedInspectorAssignmentsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectorAssignmentAsync(InspectorAssignment inspectorAssignment);
        Task<ServiceResult<bool>> UpdateAsync(InspectorAssignment inspectorAssignment);
        Task<ServiceResult<bool>> AcceptAssignmentAsync(int assignmentId, string notes = null);
        Task<ServiceResult<bool>> DeclineAssignmentAsync(int assignmentId, string declineReason, string notes = null);
        Task<ServiceResult<bool>> StartAssignmentAsync(int assignmentId);
        Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, string notes = null);
        Task<ServiceResult<bool>> ReassignAsync(int assignmentId, int newInspectorUserId, int reassignedBy, string reason = null);
        Task<ServiceResult<bool>> DeleteAsync(int assignmentId,int tenantId);
    }
}
