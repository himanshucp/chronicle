using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> GetByUsernameAsync(string username, int tenantId);
        Task<User> GetByEmailAsync(string email, int tenantId);
        Task<UserWithRoles> GetUserWithRolesAsync(int userId, int tenantId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId, int tenantId);
        Task<bool> AddUserToRoleAsync(int userId, int roleId, int tenantId);
        Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId);
        Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int tenantId);
    }
}
