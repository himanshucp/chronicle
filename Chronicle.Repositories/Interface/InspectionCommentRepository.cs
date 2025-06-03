using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionCommentRepository : IRepository<InspectionComment, int>
    {
        Task<IEnumerable<InspectionComment>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionComment>> GetByCreatedByAsync(int createdBy);
        Task<IEnumerable<InspectionComment>> GetByCommentTypeAsync(string commentType);
        Task<IEnumerable<InspectionComment>> GetByPriorityAsync(string priority);
        Task<IEnumerable<InspectionComment>> GetByStatusAsync(string status);
        Task<IEnumerable<InspectionComment>> GetInternalCommentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionComment>> GetExternalCommentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionComment>> GetCommentsRequiringResponseAsync();
        Task<IEnumerable<InspectionComment>> GetOverdueCommentsAsync();
        Task<IEnumerable<InspectionComment>> GetRepliesAsync(int parentCommentId);
        Task<IEnumerable<InspectionComment>> GetRootCommentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionComment>> GetByRespondedByAsync(int respondedBy);
        Task<IEnumerable<InspectionComment>> GetByCreatedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<InspectionComment> GetByIdAsync(int id);
        Task<IEnumerable<InspectionComment>> GetAllAsync();
        Task<PagedResult<InspectionComment>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
