using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IContractEmployeeRepository : IRepository<ContractEmployee,int>
    {
        Task<ContractEmployee> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetByContractIdAsync(int contractId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetByEmployeeIdAsync(int employeeId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetByRoleIdAsync(int roleId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetByLineManagerIdAsync(int lineManagerId, int tenantId);
        Task<ContractEmployee> GetByContractAndEmployeeAsync(int contractId, int employeeId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetAllAsync(int tenantId);
        Task<PagedResult<ContractEmployee>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<IEnumerable<ContractEmployee>> GetActiveByContractIdAsync(int contractId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetActiveByEmployeeIdAsync(int employeeId, int tenantId);
    }
}
