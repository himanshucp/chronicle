using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.UsersRepositories
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> GetByIdAsync(int id, int tenantId);
        Task<User> GetByIdWithRolesAsync(int id, int tenantId);
        Task<User> GetByUserNameAsync(string userName, int tenantId);
        Task<User> GetByEmailAsync(string email, int tenantId);
        Task<User> GetByEmployeeIdAsync(int employeeId, int tenantId);
        Task<IEnumerable<User>> GetActiveUsersAsync(int tenantId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, int tenantId);
        Task<int> InsertAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync(int tenantId);
        Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> UpdateLastLoginAsync(int userId, int tenantId);
        Task<bool> IncrementAccessFailedCountAsync(int userId, int tenantId);
        Task<bool> ResetAccessFailedCountAsync(int userId, int tenantId);
        Task<bool> SetLockoutAsync(int userId, DateTimeOffset? lockoutEnd, int tenantId);
    }
}
