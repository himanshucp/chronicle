using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.UsersRepositories
{
    public interface IUserRoleRepository : IRepository<UserRole, int>
    {
        Task<UserRole> GetByIdAsync(int id, int tenantId);
        Task<UserRole> GetByIdWithDetailsAsync(int id, int tenantId);
        Task<UserRole> GetByUserAndRoleAsync(int userId, int roleId, int tenantId);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, int tenantId);
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId, int tenantId);
        Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int tenantId);
        Task<IEnumerable<UserRole>> GetUserRolesByUserAsync(int userId, int tenantId);
        Task<IEnumerable<UserRole>> GetUsersInRoleAsync(int roleId, int tenantId);
        Task<int> InsertAsync(UserRole userRole);
        Task<bool> UpdateAsync(UserRole userRole);
        Task<IEnumerable<UserRole>> GetAllAsync(int tenantId);
        Task<PagedResult<UserRole>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId);
        Task<bool> UserIsInRoleAsync(int userId, int roleId, int tenantId);
        Task<bool> DeactivateUserRoleAsync(int userRoleId, int tenantId);
        Task<bool> ActivateUserRoleAsync(int userRoleId, int tenantId);
    }
}
