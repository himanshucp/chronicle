using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IContractEmployeeRoleService
    {
        Task<ServiceResult<ContractEmployeeRole>> GetRoleByIdAsync(int roleId, int tenantId);
        Task<ServiceResult<ContractEmployeeRole>> GetByRoleNameAsync(string roleName, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployeeRole>>> GetAllRolesAsync(int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployeeRole>>> GetActiveRolesAsync(int tenantId);
        Task<ServiceResult<PagedResult<ContractEmployeeRole>>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<int>> CreateRoleAsync(ContractEmployeeRole role, int tenantId);
        Task<ServiceResult<bool>> UpdateRoleAsync(ContractEmployeeRole role, int tenantId);
        Task<ServiceResult<bool>> DeleteRoleAsync(int roleId, int tenantId);
        Task<ServiceResult<bool>> DeactivateRoleAsync(int roleId, int tenantId);
    }
}
