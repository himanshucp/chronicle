using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public interface IUserService
    {
        Task<ServiceResult<User>> GetUserByIdAsync(int userId, int tenantId);
        Task<ServiceResult<User>> GetByUserNameAsync(string userName, int tenantId);
        Task<ServiceResult<User>> GetByEmailAsync(string email, int tenantId);
        Task<ServiceResult<User>> GetByEmployeeIdAsync(int employeeId, int tenantId);
        Task<ServiceResult<IEnumerable<User>>> GetActiveUsersAsync(int tenantId);
        Task<ServiceResult<IEnumerable<User>>> GetUsersByRoleAsync(string roleName, int tenantId);
        Task<ServiceResult<int>> CreateUserAsync(User user, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(User user, int tenantId);
        Task<ServiceResult<bool>> UpdateWithRolesAsync(User user, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int userId, int tenantId);
        Task<IEnumerable<User>> GetUsersAsync(int tenantId);
        Task<ServiceResult<PagedResult<User>>> GetPagedUsersAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<bool>> UpdateLastLoginAsync(int userId, int tenantId);
        Task<ServiceResult<bool>> IncrementAccessFailedCountAsync(int userId, int tenantId);
        Task<ServiceResult<bool>> ResetAccessFailedCountAsync(int userId, int tenantId);
        Task<ServiceResult<bool>> SetLockoutAsync(int userId, DateTimeOffset? lockoutEnd, int tenantId);
        Task<ServiceResult<bool>> ValidatePasswordAsync(string password);
        Task<ServiceResult<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, int tenantId);
    }
}
