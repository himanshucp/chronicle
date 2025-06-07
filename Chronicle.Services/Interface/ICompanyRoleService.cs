using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface ICompanyRoleService
    {
        Task<ServiceResult<CompanyRole>> GetRoleByIdAsync(int roleId, int tenantId);
        Task<ServiceResult<CompanyRole>> GetByRoleNameAsync(string roleName, int tenantId);
        Task<ServiceResult<IEnumerable<CompanyRole>>> GetAllRolesAsync(int tenantId);
        Task<ServiceResult<IEnumerable<CompanyRole>>> GetActiveRolesAsync(int tenantId);
        Task<ServiceResult<PagedResult<CompanyRole>>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<int>> CreateRoleAsync(CompanyRole role, int tenantId);
        Task<ServiceResult<bool>> UpdateRoleAsync(CompanyRole role, int tenantId);
        Task<ServiceResult<bool>> DeleteRoleAsync(int roleId, int tenantId);
        Task<ServiceResult<bool>> DeactivateRoleAsync(int roleId, int tenantId);
        Task<ServiceResult<bool>> IsRoleNameAvailableAsync(string roleName, int tenantId, int? excludeRoleId = null);
    }
}
