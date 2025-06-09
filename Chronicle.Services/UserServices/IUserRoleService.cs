using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public interface IUserRoleService
    {
        Task<ServiceResult<UserRole>> GetUserRoleByIdAsync(int userRoleId, int tenantId);
        Task<ServiceResult<UserRole>> GetByUserAndRoleAsync(int userId, int roleId, int tenantId);
        Task<ServiceResult<IEnumerable<UserRole>>> GetByUserIdAsync(int userId, int tenantId);
        Task<ServiceResult<IEnumerable<UserRole>>> GetByRoleIdAsync(int roleId, int tenantId);
        Task<ServiceResult<IEnumerable<UserRole>>> GetActiveUserRolesAsync(int tenantId);
        Task<ServiceResult<IEnumerable<UserRole>>> GetUserRolesByUserAsync(int userId, int tenantId);
        Task<ServiceResult<IEnumerable<UserRole>>> GetUsersInRoleAsync(int roleId, int tenantId);
        Task<ServiceResult<int>> CreateUserRoleAsync(UserRole userRole, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(UserRole userRole, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int userRoleId, int tenantId);
        Task<ServiceResult<bool>> AddUserToRoleAsync(int userId, int roleId, int tenantId);
        Task<ServiceResult<bool>> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId);
        Task<ServiceResult<bool>> UserIsInRoleAsync(int userId, int roleId, int tenantId);
        Task<ServiceResult<bool>> DeactivateUserRoleAsync(int userRoleId, int tenantId);
        Task<ServiceResult<bool>> ActivateUserRoleAsync(int userRoleId, int tenantId);
        Task<ServiceResult<bool>> AddUserToMultipleRolesAsync(int userId, List<int> roleIds, int tenantId);
        Task<ServiceResult<bool>> RemoveUserFromMultipleRolesAsync(int userId, List<int> roleIds, int tenantId);
        Task<ServiceResult<bool>> UpdateUserRolesAsync(int userId, List<int> newRoleIds, int tenantId);
        Task<IEnumerable<UserRole>> GetUserRolesAsync(int tenantId);
        Task<ServiceResult<PagedResult<UserRole>>> GetPagedUserRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
    }
}
