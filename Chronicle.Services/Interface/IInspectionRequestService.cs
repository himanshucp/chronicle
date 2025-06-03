using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionRequestService
    {
        Task<ServiceResult<InspectionRequest>> GetInspectionRequestByIdAsync(int inspectionRequestId, int tenantId);
        Task<ServiceResult<InspectionRequest>> GetByIRNumberAsync(string irNumber, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByContractIdAsync(int contractId, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByIssuerUserIdAsync(int issuerUserId, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByIssuingCompanyIdAsync(int issuingCompanyId, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByDisciplineIdAsync(int disciplineId, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByInspectionDateRangeAsync(DateTime fromDate, DateTime toDate, int tenantId);
        Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByCurrentStatusAsync(string status, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetInspectionRequestsAsync(int tenantId);
        Task<ServiceResult<PagedResult<InspectionRequest>>> GetPagedInspectionRequestsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionRequestAsync(InspectionRequest inspectionRequest, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(InspectionRequest inspectionRequest, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int inspectionRequestId, int tenantId);
    }
}
