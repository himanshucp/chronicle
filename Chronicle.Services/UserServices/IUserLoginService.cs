using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public interface IUserLoginService
    {
        Task<ServiceResult<UserLogin>> GetUserLoginByIdAsync(int userLoginId, int tenantId);
        Task<ServiceResult<UserLogin>> GetByProviderAsync(string loginProvider, string providerKey, int tenantId);
        Task<ServiceResult<IEnumerable<UserLogin>>> GetByUserIdAsync(int userId, int tenantId);
        Task<ServiceResult<IEnumerable<UserLogin>>> GetByLoginProviderAsync(string loginProvider, int tenantId);
        Task<ServiceResult<UserLogin>> GetUserLoginAsync(int userId, string loginProvider, int tenantId);
        Task<ServiceResult<int>> CreateUserLoginAsync(UserLogin userLogin, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(UserLogin userLogin, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int userLoginId, int tenantId);
        Task<ServiceResult<bool>> DeleteByUserAndProviderAsync(int userId, string loginProvider, int tenantId);
        Task<ServiceResult<bool>> AddLoginToUserAsync(int userId, string loginProvider, string providerKey, string providerDisplayName, int tenantId);
        Task<ServiceResult<bool>> RemoveLoginFromUserAsync(int userId, string loginProvider, int tenantId);
        Task<ServiceResult<bool>> ExistsAsync(string loginProvider, string providerKey, int tenantId);
        Task<ServiceResult<User>> FindUserByLoginAsync(string loginProvider, string providerKey, int tenantId);
        Task<ServiceResult<IEnumerable<string>>> GetLoginProvidersAsync(int tenantId);
        Task<ServiceResult<int>> GetUserLoginCountAsync(int userId, int tenantId);
        Task<IEnumerable<UserLogin>> GetUserLoginsAsync(int tenantId);
        Task<ServiceResult<PagedResult<UserLogin>>> GetPagedUserLoginsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<bool>> ValidateLoginProviderAsync(string loginProvider, string providerKey);
        Task<ServiceResult<bool>> CanAddLoginToUserAsync(int userId, string loginProvider, int tenantId);
    }
}
