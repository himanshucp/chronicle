using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IContractRepository : IRepository<Contract,int>
    {
        Task<Contract> GetByIdAsync(int id, int tenantId);
        Task<Contract> GetByIdWithEmployeesAsync(int id, int tenantId); // NEW METHOD
        Task<Contract> GetByExternalIdAsync(string contractExternalId, int tenantId);
        Task<Contract> GetByTitleAsync(string contractTitle, int tenantId);
        Task<IEnumerable<Contract>> GetContractsByProjectAsync(int projectId, int tenantId);
        Task<IEnumerable<Contract>> GetContractsByCompanyAsync(int companyId, int tenantId);
        Task<IEnumerable<Contract>> GetChildContractsAsync(int parentContractId, int tenantId);
        Task<int> InsertAsync(Contract contract);
        Task<bool> UpdateAsync(Contract contract);
        Task<IEnumerable<Contract>> GetAllAsync(int tenantId);
        Task<PagedResult<Contract>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);

        Task<Contract> GetByInspectionAgencyContracAsync(string inspectionAgencyContract, int tenantId);

        Task<Contract> GetByManagingAgencyContractAsync(string managingAgencyContract, int tenantId);


    }
}
