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
        Task<Contract> GetByExternalIdAsync(string contractExternalId, int tenantId);
        Task<Contract> GetByTitleAsync(string contractTitle, int tenantId);
        Task<IEnumerable<Contract>> GetContractsByProjectAsync(int projectId, int tenantId);
        Task<IEnumerable<Contract>> GetContractsByCompanyAsync(int companyId, int tenantId);
        Task<IEnumerable<Contract>> GetChildContractsAsync(int parentContractId, int tenantId);
        Task<Contract> GetByIdAsync(int id, int tenantId);

        Task<IEnumerable<Contract>> GetAllAsync(int tenantId);
        Task<PagedResult<Contract>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);


    }
}
