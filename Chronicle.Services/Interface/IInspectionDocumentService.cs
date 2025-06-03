using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionDocumentService
    {
        Task<ServiceResult<InspectionDocument>> GetInspectionDocumentByIdAsync(int documentId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByDocumentTypeAsync(string documentType);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByDocumentCategoryAsync(string documentCategory);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByUploadedByAsync(int uploadedBy);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByApprovedByAsync(int approvedBy);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetRequiredDocumentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetOptionalDocumentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetPendingApprovalDocumentsAsync();
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetApprovedDocumentsAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByUploadedDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetDocumentVersionsAsync(string documentName);
        Task<ServiceResult<InspectionDocument>> GetLatestVersionAsync(string documentName, int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByMimeTypeAsync(string mimeType);
        Task<ServiceResult<long>> GetTotalFileSizeByInspectionRequestAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionDocument>> GetInspectionDocumentsAsync();
        Task<ServiceResult<PagedResult<InspectionDocument>>> GetPagedInspectionDocumentsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionDocumentAsync(InspectionDocument inspectionDocument);
        Task<ServiceResult<bool>> UpdateAsync(InspectionDocument inspectionDocument);
        Task<ServiceResult<bool>> ApproveDocumentAsync(int documentId, int approvedBy);
        Task<ServiceResult<bool>> RejectDocumentAsync(int documentId, string rejectionReason);
        Task<ServiceResult<int>> CreateNewVersionAsync(int originalDocumentId, InspectionDocument newVersionDocument);
        Task<ServiceResult<bool>> MarkAsRequiredAsync(int documentId, bool isRequired);
        Task<ServiceResult<bool>> DeleteAsync(int documentId, int tenantId);
    }
}
