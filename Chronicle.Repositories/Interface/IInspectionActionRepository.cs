using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionActionRepository : IRepository<InspectionAction, int>
    {
        Task<IEnumerable<InspectionAction>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionAction>> GetByInspectorUserIdAsync(int inspectorUserId);
        Task<IEnumerable<InspectionAction>> GetByActionTypeAsync(string actionType);
        Task<IEnumerable<InspectionAction>> GetByInspectionResultAsync(string inspectionResult);
        Task<IEnumerable<InspectionAction>> GetByComplianceStatusAsync(string complianceStatus);
        Task<IEnumerable<InspectionAction>> GetByActionDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectionAction>> GetPendingActionsAsync();
        Task<IEnumerable<InspectionAction>> GetCompletedActionsAsync();
        Task<IEnumerable<InspectionAction>> GetActionsRequiringFollowUpAsync();
        Task<IEnumerable<InspectionAction>> GetActionsRequiringReviewAsync();
        Task<IEnumerable<InspectionAction>> GetByReviewedByAsync(int reviewedBy);
        Task<InspectionAction> GetByIdAsync(int id);
        Task<IEnumerable<InspectionAction>> GetAllAsync();
        Task<PagedResult<InspectionAction>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
