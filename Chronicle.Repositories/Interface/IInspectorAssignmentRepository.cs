using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectorAssignmentRepository : IRepository<InspectorAssignment, int>
    {
        Task<IEnumerable<InspectorAssignment>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectorAssignment>> GetByInspectorUserIdAsync(int inspectorUserId);
        Task<IEnumerable<InspectorAssignment>> GetByAssignedByAsync(int assignedBy);
        Task<IEnumerable<InspectorAssignment>> GetByAssignmentTypeAsync(string assignmentType);
        Task<IEnumerable<InspectorAssignment>> GetByStatusAsync(string status);
        Task<IEnumerable<InspectorAssignment>> GetByAssignedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectorAssignment>> GetPendingAssignmentsAsync();
        Task<IEnumerable<InspectorAssignment>> GetAcceptedAssignmentsAsync();
        Task<IEnumerable<InspectorAssignment>> GetDeclinedAssignmentsAsync();
        Task<IEnumerable<InspectorAssignment>> GetCompletedAssignmentsAsync();
        Task<IEnumerable<InspectorAssignment>> GetOverdueAssignmentsAsync();
        Task<IEnumerable<InspectorAssignment>> GetActiveAssignmentsForInspectorAsync(int inspectorUserId);
        Task<InspectorAssignment> GetActiveAssignmentForInspectionRequestAsync(int inspectionRequestId);
        Task<InspectorAssignment> GetByIdAsync(int id);
        Task<IEnumerable<InspectorAssignment>> GetAllAsync();
        Task<PagedResult<InspectorAssignment>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
