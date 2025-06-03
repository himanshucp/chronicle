using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionRequestRepository : IRepository<InspectionRequest, int>
    {
        Task<InspectionRequest> GetByIRNumberAsync(string irNumber, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByContractIdAsync(int contractId, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByIssuerUserIdAsync(int issuerUserId, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByIssuingCompanyIdAsync(int issuingCompanyId, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByDisciplineIdAsync(int disciplineId, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByInspectionDateRangeAsync(DateTime fromDate, DateTime toDate, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetByCurrentStatusAsync(string status, int tenantId);
        Task<InspectionRequest> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<InspectionRequest>> GetAllAsync(int tenantId);
        Task<PagedResult<InspectionRequest>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
    }
}
