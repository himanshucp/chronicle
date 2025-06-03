using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionCommentService
    {
        Task<ServiceResult<InspectionComment>> GetInspectionCommentByIdAsync(int commentId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCreatedByAsync(int createdBy);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCommentTypeAsync(string commentType);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByPriorityAsync(string priority);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByStatusAsync(string status);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetInternalCommentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetExternalCommentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetCommentsRequiringResponseAsync();
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetOverdueCommentsAsync();
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetRepliesAsync(int parentCommentId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetRootCommentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByRespondedByAsync(int respondedBy);
        Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCreatedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectionComment>> GetInspectionCommentsAsync();
        Task<ServiceResult<PagedResult<InspectionComment>>> GetPagedInspectionCommentsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionCommentAsync(InspectionComment inspectionComment);
        Task<ServiceResult<int>> ReplyToCommentAsync(int parentCommentId, InspectionComment replyComment);
        Task<ServiceResult<bool>> UpdateAsync(InspectionComment inspectionComment);
        Task<ServiceResult<bool>> MarkAsRespondedAsync(int commentId, int respondedBy);
        Task<ServiceResult<bool>> CloseCommentAsync(int commentId, int closedBy);
        Task<ServiceResult<bool>> ReopenCommentAsync(int commentId, int reopenedBy);
        Task<ServiceResult<bool>> DeleteAsync(int commentId, int tenantId);
    }
}
