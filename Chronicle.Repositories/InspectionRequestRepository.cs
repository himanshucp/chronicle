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
    public class InspectionRequestRepository : DapperRepository<InspectionRequest, int>, IInspectionRequestRepository
    {
        public InspectionRequestRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionRequests", "InspectionRequestID")
        {
        }

        public async Task<InspectionRequest> GetByIRNumberAsync(string irNumber, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE IRNumber = @IRNumber AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionRequest>(
                sql,
                new { IRNumber = irNumber, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByContractIdAsync(int contractId, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE ContractID = @ContractID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { ContractID = contractId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByIssuerUserIdAsync(int issuerUserId, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE IssuerUserID = @IssuerUserID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { IssuerUserID = issuerUserId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByIssuingCompanyIdAsync(int issuingCompanyId, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE IssuingCompanyID = @IssuingCompanyID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { IssuingCompanyID = issuingCompanyId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByDisciplineIdAsync(int disciplineId, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE DisciplineID = @DisciplineID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { DisciplineID = disciplineId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByInspectionDateRangeAsync(DateTime fromDate, DateTime toDate, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE InspectionDate >= @FromDate AND InspectionDate <= @ToDate AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { FromDate = fromDate, ToDate = toDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionRequest>> GetByCurrentStatusAsync(string status, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE CurrentStatus = @CurrentStatus AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { CurrentStatus = status, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionRequest> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE InspectionRequestID = @InspectionRequestID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionRequest>(
                sql,
                new { InspectionRequestID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionRequest inspectionRequest)
        {
            const string sql = @"
                INSERT INTO InspectionRequests (
                    TenantID, IRNumber, ContractID, IssuerUserID, IssuingCompanyID, RecipientCompanyID, 
                    RecipientPMCID, RecipientClientID, DisciplineID, SubDisciplineID, Location, AreaUnit, 
                    ChainageFrom, ChainageTo, FloorLevel, AssetTagNumber, SystemRoom, SubsystemRoom, 
                    EarthworksLayerNumber, BOQReferenceNumber, WBSCode, SnagNumbers, IsSnagWalkthrough, 
                    IsDesnaggingInspection, InspectionDescription, ITPID, ActivityID, InspectionInterventionPMC, 
                    InspectionInterventionClient, PreviousIRNumbers, InspectionDate, InspectionTime, 
                    ExpectedDuration, Quantity, Unit, Location3D, GPSCoordinates, WorkflowInstanceID, 
                    CurrentStatus, PersonResponsible, SubmissionApprovedBy, SubmissionApprovedDate, 
                    SubmissionComments, CreatedDate, ModifiedDate, CreatedBy, ModifiedBy, IsActive)
                VALUES (
                    @TenantID, @IRNumber, @ContractID, @IssuerUserID, @IssuingCompanyID, @RecipientCompanyID, 
                    @RecipientPMCID, @RecipientClientID, @DisciplineID, @SubDisciplineID, @Location, @AreaUnit, 
                    @ChainageFrom, @ChainageTo, @FloorLevel, @AssetTagNumber, @SystemRoom, @SubsystemRoom, 
                    @EarthworksLayerNumber, @BOQReferenceNumber, @WBSCode, @SnagNumbers, @IsSnagWalkthrough, 
                    @IsDesnaggingInspection, @InspectionDescription, @ITPID, @ActivityID, @InspectionInterventionPMC, 
                    @InspectionInterventionClient, @PreviousIRNumbers, @InspectionDate, @InspectionTime, 
                    @ExpectedDuration, @Quantity, @Unit, @Location3D, @GPSCoordinates, @WorkflowInstanceID, 
                    @CurrentStatus, @PersonResponsible, @SubmissionApprovedBy, @SubmissionApprovedDate, 
                    @SubmissionComments, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy, 1);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (inspectionRequest.CreatedDate == default(DateTime))
            {
                inspectionRequest.CreatedDate = DateTime.UtcNow;
                inspectionRequest.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionRequest,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionRequest inspectionRequest)
        {
            const string sql = @"
                UPDATE InspectionRequests
                SET IRNumber = @IRNumber,
                    ContractID = @ContractID,
                    IssuerUserID = @IssuerUserID,
                    IssuingCompanyID = @IssuingCompanyID,
                    RecipientCompanyID = @RecipientCompanyID,
                    RecipientPMCID = @RecipientPMCID,
                    RecipientClientID = @RecipientClientID,
                    DisciplineID = @DisciplineID,
                    SubDisciplineID = @SubDisciplineID,
                    Location = @Location,
                    AreaUnit = @AreaUnit,
                    ChainageFrom = @ChainageFrom,
                    ChainageTo = @ChainageTo,
                    FloorLevel = @FloorLevel,
                    AssetTagNumber = @AssetTagNumber,
                    SystemRoom = @SystemRoom,
                    SubsystemRoom = @SubsystemRoom,
                    EarthworksLayerNumber = @EarthworksLayerNumber,
                    BOQReferenceNumber = @BOQReferenceNumber,
                    WBSCode = @WBSCode,
                    SnagNumbers = @SnagNumbers,
                    IsSnagWalkthrough = @IsSnagWalkthrough,
                    IsDesnaggingInspection = @IsDesnaggingInspection,
                    InspectionDescription = @InspectionDescription,
                    ITPID = @ITPID,
                    ActivityID = @ActivityID,
                    InspectionInterventionPMC = @InspectionInterventionPMC,
                    InspectionInterventionClient = @InspectionInterventionClient,
                    PreviousIRNumbers = @PreviousIRNumbers,
                    InspectionDate = @InspectionDate,
                    InspectionTime = @InspectionTime,
                    ExpectedDuration = @ExpectedDuration,
                    Quantity = @Quantity,
                    Unit = @Unit,
                    Location3D = @Location3D,
                    GPSCoordinates = @GPSCoordinates,
                    WorkflowInstanceID = @WorkflowInstanceID,
                    CurrentStatus = @CurrentStatus,
                    PersonResponsible = @PersonResponsible,
                    SubmissionApprovedBy = @SubmissionApprovedBy,
                    SubmissionApprovedDate = @SubmissionApprovedDate,
                    SubmissionComments = @SubmissionComments,
                    ModifiedDate = @ModifiedDate,
                    ModifiedBy = @ModifiedBy
                WHERE InspectionRequestID = @InspectionRequestID AND TenantID = @TenantID";

            // Set modification date
            inspectionRequest.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionRequest,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionRequest>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM InspectionRequests WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<InspectionRequest>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionRequest>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    IRNumber LIKE @SearchTerm OR 
                    InspectionDescription LIKE @SearchTerm OR
                    Location LIKE @SearchTerm OR
                    CurrentStatus LIKE @SearchTerm OR
                    PersonResponsible LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionRequest>(
                "InspectionRequests",
                "CreatedDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM InspectionRequests WHERE InspectionRequestID = @InspectionRequestID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { InspectionRequestID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
