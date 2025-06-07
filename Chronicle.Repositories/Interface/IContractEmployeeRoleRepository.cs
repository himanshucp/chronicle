using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IContractEmployeeRoleRepository : IRepository<ContractEmployeeRole, int>
    {
        Task<ContractEmployeeRole> GetByRoleNameAsync(string roleName, int tenantId);
        Task<ContractEmployeeRole> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<ContractEmployeeRole>> GetAllAsync(int tenantId);
        Task<IEnumerable<ContractEmployeeRole>> GetActiveRolesAsync(int tenantId);
        Task<PagedResult<ContractEmployeeRole>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
    }
}
