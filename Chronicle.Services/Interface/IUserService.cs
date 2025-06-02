using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    /// <summary>
    /// Service for user management
    /// </summary>
    public interface IUserService
    {
        public interface IUserService
        {
            Task<User> GetUserByIdAsync(int id, int tenantId);
            Task<IEnumerable<User>> GetAllUsersAsync(int tenantId);
            Task<User> GetUserByUsernameAsync(string username, int tenantId);
            Task<User> GetUserByEmailAsync(string email, int tenantId);
            Task<UserWithRoles> GetUserWithRolesAsync(int userId, int tenantId);
            Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId, int tenantId);
            Task<int> CreateUserAsync(User user);
            Task<bool> UpdateUserAsync(User user);
            Task<bool> DeleteUserAsync(int id, int tenantId);
            Task<bool> AddUserToRoleAsync(int userId, int roleId, int tenantId);
            Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId);
            Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int tenantId);
            Task<PagedResult<User>> GetPagedUsersAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        }

    }

}
