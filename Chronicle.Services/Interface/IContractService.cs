using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IContractService
    {
      
        Task<ServiceResult<Contract>> GetContractByIdAsync(int contractId, int tenantId);
        Task<ServiceResult<Contract>> GetByExternalIdAsync(string contractExternalId, int tenantId);
        Task<ServiceResult<Contract>> GetByTitleAsync(string contractTitle, int tenantId);
        Task<ServiceResult<IEnumerable<Contract>>> GetContractsByProjectAsync(int projectId, int tenantId);
        Task<ServiceResult<IEnumerable<Contract>>> GetContractsByCompanyAsync(int companyId, int tenantId);
        Task<ServiceResult<IEnumerable<Contract>>> GetChildContractsAsync(int parentContractId, int tenantId);
        Task<ServiceResult<int>> CreateContractAsync(Contract contract, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(Contract contract, int tenantId);
        Task<ServiceResult<bool>> UpdateWithEmployeesAsync(Contract contract, int tenantId); // NEW METHOD
        Task<ServiceResult<bool>> DeleteAsync(int contractId, int tenantId);
        Task<IEnumerable<Contract>> GetContractsAsync(int tenantId);
        Task<ServiceResult<PagedResult<Contract>>> GetPagedContractsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
    }
}
