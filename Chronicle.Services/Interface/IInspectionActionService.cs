using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionActionService
    {
        Task<ServiceResult<InspectionAction>> GetInspectionActionByIdAsync(int actionId);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectorUserIdAsync(int inspectorUserId);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByActionTypeAsync(string actionType);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectionResultAsync(string inspectionResult);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByComplianceStatusAsync(string complianceStatus);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByActionDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetPendingActionsAsync();
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetCompletedActionsAsync();
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetActionsRequiringFollowUpAsync();
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetActionsRequiringReviewAsync();
        Task<ServiceResult<IEnumerable<InspectionAction>>> GetByReviewedByAsync(int reviewedBy);
        Task<IEnumerable<InspectionAction>> GetInspectionActionsAsync();
        Task<ServiceResult<PagedResult<InspectionAction>>> GetPagedInspectionActionsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionActionAsync(InspectionAction inspectionAction);
        Task<ServiceResult<bool>> UpdateAsync(InspectionAction inspectionAction);
        Task<ServiceResult<bool>> CompleteActionAsync(int actionId, string completionComments = null);
        Task<ServiceResult<bool>> ReviewActionAsync(int actionId, int reviewedBy, string reviewComments = null, bool approved = true);
        Task<ServiceResult<bool>> DeleteAsync(int actionId, int tenantId);
    }
}
