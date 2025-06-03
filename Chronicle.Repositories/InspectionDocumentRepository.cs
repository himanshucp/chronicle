using Chronicle.Data.Extensions;
using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public class InspectionDocumentRepository : DapperRepository<InspectionDocument, int>, IInspectionDocumentRepository
    {
        public InspectionDocumentRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionDocuments", "DocumentID")
        {
        }

        public async Task<IEnumerable<InspectionDocument>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE InspectionRequestID = @InspectionRequestID AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByDocumentTypeAsync(string documentType)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE DocumentType = @DocumentType AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { DocumentType = documentType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByDocumentCategoryAsync(string documentCategory)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE DocumentCategory = @DocumentCategory AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { DocumentCategory = documentCategory },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByUploadedByAsync(int uploadedBy)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE UploadedBy = @UploadedBy AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { UploadedBy = uploadedBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByApprovedByAsync(int approvedBy)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE ApprovedBy = @ApprovedBy AND IsActive = 1 ORDER BY ApprovedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { ApprovedBy = approvedBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetRequiredDocumentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE InspectionRequestID = @InspectionRequestID AND IsRequired = 1 AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetOptionalDocumentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE InspectionRequestID = @InspectionRequestID AND IsRequired = 0 AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetPendingApprovalDocumentsAsync()
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE ApprovedBy IS NULL AND ApprovedDate IS NULL AND IsActive = 1 ORDER BY UploadedDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetApprovedDocumentsAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE InspectionRequestID = @InspectionRequestID AND ApprovedBy IS NOT NULL AND ApprovedDate IS NOT NULL AND IsActive = 1 ORDER BY ApprovedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByUploadedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE UploadedDate >= @FromDate AND UploadedDate <= @ToDate AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByDocumentVersionAsync(string documentName, string version)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE DocumentName = @DocumentName AND DocumentVersion = @DocumentVersion AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { DocumentName = documentName, DocumentVersion = version },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetDocumentVersionsAsync(string documentName)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE DocumentName = @DocumentName AND IsActive = 1 ORDER BY DocumentVersion DESC, UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { DocumentName = documentName },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionDocument> GetLatestVersionAsync(string documentName, int inspectionRequestId)
        {
            const string sql = @"
                SELECT TOP 1 * FROM InspectionDocuments 
                WHERE DocumentName = @DocumentName 
                AND InspectionRequestID = @InspectionRequestID 
                AND IsActive = 1 
                ORDER BY DocumentVersion DESC, UploadedDate DESC";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionDocument>(
                sql,
                new { DocumentName = documentName, InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionDocument>> GetByMimeTypeAsync(string mimeType)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE MimeType = @MimeType AND IsActive = 1 ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                new { MimeType = mimeType },
                _unitOfWork.Transaction);
        }

        public async Task<long> GetTotalFileSizeByInspectionRequestAsync(int inspectionRequestId)
        {
            const string sql = "SELECT ISNULL(SUM(FileSize), 0) FROM InspectionDocuments WHERE InspectionRequestID = @InspectionRequestID AND IsActive = 1";
            return await _unitOfWork.Connection.QuerySingleAsync<long>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionDocument> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectionDocuments WHERE DocumentID = @DocumentID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionDocument>(
                sql,
                new { DocumentID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionDocument inspectionDocument)
        {
            const string sql = @"
                INSERT INTO InspectionDocuments (
                    InspectionRequestID, DocumentName, DocumentType, DocumentCategory, DocumentPath, 
                    DocumentUrl, FileSize, MimeType, DocumentVersion, Description, IsRequired, 
                    UploadedDate, UploadedBy, ApprovedBy, ApprovedDate, IsActive)
                VALUES (
                    @InspectionRequestID, @DocumentName, @DocumentType, @DocumentCategory, @DocumentPath, 
                    @DocumentUrl, @FileSize, @MimeType, @DocumentVersion, @Description, @IsRequired, 
                    @UploadedDate, @UploadedBy, @ApprovedBy, @ApprovedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set upload date if not set
            if (inspectionDocument.UploadedDate == default(DateTime))
            {
                inspectionDocument.UploadedDate = DateTime.UtcNow;
            }

            // Set default version if not set
            if (string.IsNullOrEmpty(inspectionDocument.DocumentVersion))
            {
                inspectionDocument.DocumentVersion = "1.0";
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionDocument,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionDocument inspectionDocument)
        {
            const string sql = @"
                UPDATE InspectionDocuments
                SET InspectionRequestID = @InspectionRequestID,
                    DocumentName = @DocumentName,
                    DocumentType = @DocumentType,
                    DocumentCategory = @DocumentCategory,
                    DocumentPath = @DocumentPath,
                    DocumentUrl = @DocumentUrl,
                    FileSize = @FileSize,
                    MimeType = @MimeType,
                    DocumentVersion = @DocumentVersion,
                    Description = @Description,
                    IsRequired = @IsRequired,
                    UploadedDate = @UploadedDate,
                    UploadedBy = @UploadedBy,
                    ApprovedBy = @ApprovedBy,
                    ApprovedDate = @ApprovedDate,
                    IsActive = @IsActive
                WHERE DocumentID = @DocumentID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionDocument,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionDocument>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectionDocuments ORDER BY UploadedDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionDocument>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionDocument>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    DocumentName LIKE @SearchTerm OR 
                    DocumentType LIKE @SearchTerm OR
                    DocumentCategory LIKE @SearchTerm OR
                    Description LIKE @SearchTerm OR
                    MimeType LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionDocument>(
                "InspectionDocuments",
                "UploadedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectionDocuments WHERE DocumentID = @DocumentID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { DocumentID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
