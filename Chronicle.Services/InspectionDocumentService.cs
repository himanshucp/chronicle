using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class InspectionDocumentService : IInspectionDocumentService
    {
        private readonly IInspectionDocumentRepository _inspectionDocumentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionDocumentService(
            IInspectionDocumentRepository inspectionDocumentRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionDocumentRepository = inspectionDocumentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionDocument>> GetInspectionDocumentByIdAsync(int documentId)
        {
            try
            {
                var inspectionDocument = await _inspectionDocumentRepository.GetByIdAsync(documentId);
                if (inspectionDocument == null)
                {
                    return ServiceResult<InspectionDocument>.FailureResult("Inspection document not found");
                }

                return ServiceResult<InspectionDocument>.SuccessResult(inspectionDocument);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionDocument>.FailureResult($"Error retrieving inspection document: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByDocumentTypeAsync(string documentType)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByDocumentTypeAsync(documentType);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByDocumentCategoryAsync(string documentCategory)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByDocumentCategoryAsync(documentCategory);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByUploadedByAsync(int uploadedBy)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByUploadedByAsync(uploadedBy);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByApprovedByAsync(int approvedBy)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByApprovedByAsync(approvedBy);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetRequiredDocumentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetRequiredDocumentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving required inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetOptionalDocumentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetOptionalDocumentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving optional inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetPendingApprovalDocumentsAsync()
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetPendingApprovalDocumentsAsync();
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving pending approval documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetApprovedDocumentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetApprovedDocumentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving approved inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByUploadedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByUploadedDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetDocumentVersionsAsync(string documentName)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetDocumentVersionsAsync(documentName);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving document versions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<InspectionDocument>> GetLatestVersionAsync(string documentName, int inspectionRequestId)
        {
            try
            {
                var inspectionDocument = await _inspectionDocumentRepository.GetLatestVersionAsync(documentName, inspectionRequestId);
                if (inspectionDocument == null)
                {
                    return ServiceResult<InspectionDocument>.FailureResult("Latest document version not found");
                }

                return ServiceResult<InspectionDocument>.SuccessResult(inspectionDocument);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionDocument>.FailureResult($"Error retrieving latest document version: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionDocument>>> GetByMimeTypeAsync(string mimeType)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetByMimeTypeAsync(mimeType);
                return ServiceResult<IEnumerable<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }

        public async Task<ServiceResult<long>> GetTotalFileSizeByInspectionRequestAsync(int inspectionRequestId)
        {
            try
            {
                var totalSize = await _inspectionDocumentRepository.GetTotalFileSizeByInspectionRequestAsync(inspectionRequestId);
                return ServiceResult<long>.SuccessResult(totalSize);
            }
            catch (Exception ex)
            {
                return ServiceResult<long>.FailureResult($"Error calculating total file size: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionDocumentAsync(InspectionDocument inspectionDocument)
        {
            try
            {
                // Set default values
                inspectionDocument.UploadedDate = DateTime.UtcNow;
                inspectionDocument.IsActive = true;

                if (string.IsNullOrEmpty(inspectionDocument.DocumentVersion))
                {
                    inspectionDocument.DocumentVersion = "1.0";
                }

                _unitOfWork.BeginTransaction();

                int documentId = await _inspectionDocumentRepository.InsertAsync(inspectionDocument);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(documentId, "Inspection document created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection document: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionDocument inspectionDocument)
        {
            try
            {
                // Check if inspection document exists
                var existingInspectionDocument = await _inspectionDocumentRepository.GetByIdAsync(inspectionDocument.DocumentID);
                if (existingInspectionDocument == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection document not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionDocumentRepository.UpdateAsync(inspectionDocument);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection document updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection document: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ApproveDocumentAsync(int documentId, int approvedBy)
        {
            try
            {
                var existingDocument = await _inspectionDocumentRepository.GetByIdAsync(documentId);
                if (existingDocument == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection document not found");
                }

                if (existingDocument.ApprovedBy.HasValue && existingDocument.ApprovedDate.HasValue)
                {
                    return ServiceResult<bool>.FailureResult("Document has already been approved");
                }

                existingDocument.ApprovedBy = approvedBy;
                existingDocument.ApprovedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionDocumentRepository.UpdateAsync(existingDocument);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Document approved successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error approving document: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RejectDocumentAsync(int documentId, string rejectionReason)
        {
            try
            {
                var existingDocument = await _inspectionDocumentRepository.GetByIdAsync(documentId);
                if (existingDocument == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection document not found");
                }

                // Clear approval fields to indicate rejection
                existingDocument.ApprovedBy = null;
                existingDocument.ApprovedDate = null;

                // Add rejection reason to description
                if (!string.IsNullOrEmpty(rejectionReason))
                {
                    existingDocument.Description = string.IsNullOrEmpty(existingDocument.Description)
                        ? $"REJECTED: {rejectionReason}"
                        : $"{existingDocument.Description}\n\nREJECTED: {rejectionReason}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionDocumentRepository.UpdateAsync(existingDocument);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Document rejected successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error rejecting document: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateNewVersionAsync(int originalDocumentId, InspectionDocument newVersionDocument)
        {
            try
            {
                var originalDocument = await _inspectionDocumentRepository.GetByIdAsync(originalDocumentId);
                if (originalDocument == null)
                {
                    return ServiceResult<int>.FailureResult("Original document not found");
                }

                // Inherit properties from original document
                newVersionDocument.InspectionRequestID = originalDocument.InspectionRequestID;
                newVersionDocument.DocumentName = originalDocument.DocumentName;
                newVersionDocument.DocumentType = originalDocument.DocumentType;
                newVersionDocument.DocumentCategory = originalDocument.DocumentCategory;
                newVersionDocument.IsRequired = originalDocument.IsRequired;

                // Generate new version number
                var existingVersions = await _inspectionDocumentRepository.GetDocumentVersionsAsync(originalDocument.DocumentName);
                var maxVersion = existingVersions
                    .Where(d => !string.IsNullOrEmpty(d.DocumentVersion))
                    .Select(d =>
                    {
                        if (decimal.TryParse(d.DocumentVersion, out decimal version))
                            return version;
                        return 0m;
                    })
                    .DefaultIfEmpty(0m)
                    .Max();

                newVersionDocument.DocumentVersion = (maxVersion + 0.1m).ToString("F1");

                // Set upload properties
                newVersionDocument.UploadedDate = DateTime.UtcNow;
                newVersionDocument.IsActive = true;
                newVersionDocument.ApprovedBy = null;
                newVersionDocument.ApprovedDate = null;

                _unitOfWork.BeginTransaction();

                int newDocumentId = await _inspectionDocumentRepository.InsertAsync(newVersionDocument);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(newDocumentId, "New document version created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating new document version: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsRequiredAsync(int documentId, bool isRequired)
        {
            try
            {
                var existingDocument = await _inspectionDocumentRepository.GetByIdAsync(documentId);
                if (existingDocument == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection document not found");
                }

                existingDocument.IsRequired = isRequired;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionDocumentRepository.UpdateAsync(existingDocument);

                _unitOfWork.Commit();

                string message = isRequired ? "Document marked as required successfully" : "Document marked as optional successfully";
                return ServiceResult<bool>.SuccessResult(result, message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating document requirement status: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int documentId,int tenantId)
        {
            try
            {
                // Check if inspection document exists
                var existingInspectionDocument = await _inspectionDocumentRepository.GetByIdAsync(documentId);
                if (existingInspectionDocument == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection document not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionDocumentRepository.DeleteAsync(documentId,tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection document deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection document: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionDocument>> GetInspectionDocumentsAsync()
        {
            return await _inspectionDocumentRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectionDocument>>> GetPagedInspectionDocumentsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectionDocuments = await _inspectionDocumentRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectionDocument>>.SuccessResult(inspectionDocuments);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionDocument>>.FailureResult($"Error retrieving inspection documents: {ex.Message}");
            }
        }
    }
}
