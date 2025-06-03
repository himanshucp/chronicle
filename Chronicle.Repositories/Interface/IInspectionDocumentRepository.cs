using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionDocumentRepository : IRepository<InspectionDocument, int>
    {
        Task<IEnumerable<InspectionDocument>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetByDocumentTypeAsync(string documentType);
        Task<IEnumerable<InspectionDocument>> GetByDocumentCategoryAsync(string documentCategory);
        Task<IEnumerable<InspectionDocument>> GetByUploadedByAsync(int uploadedBy);
        Task<IEnumerable<InspectionDocument>> GetByApprovedByAsync(int approvedBy);
        Task<IEnumerable<InspectionDocument>> GetRequiredDocumentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetOptionalDocumentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetPendingApprovalDocumentsAsync();
        Task<IEnumerable<InspectionDocument>> GetApprovedDocumentsAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetByUploadedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectionDocument>> GetByDocumentVersionAsync(string documentName, string version);
        Task<IEnumerable<InspectionDocument>> GetDocumentVersionsAsync(string documentName);
        Task<InspectionDocument> GetLatestVersionAsync(string documentName, int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetByMimeTypeAsync(string mimeType);
        Task<long> GetTotalFileSizeByInspectionRequestAsync(int inspectionRequestId);
        Task<InspectionDocument> GetByIdAsync(int id);
        Task<IEnumerable<InspectionDocument>> GetAllAsync();
        Task<PagedResult<InspectionDocument>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
