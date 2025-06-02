using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    /// <summary>
    /// Service for role management
    /// </summary>
    public interface IRoleService
    {
        Task<Role> GetRoleByIdAsync(int id, int tenantId);
        Task<IEnumerable<Role>> GetAllRolesAsync(int tenantId);
        Task<Role> GetRoleByNameAsync(string name, int tenantId);
        Task<IEnumerable<Role>> GetRolesByUserAsync(int userId, int tenantId);
        Task<int> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id, int tenantId);
        Task<PagedResult<Role>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
    }
}
