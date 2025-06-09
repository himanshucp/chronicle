using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.UsersRepositories
{
    public interface IUserLoginRepository : IRepository<UserLogin, int>
    {
        Task<UserLogin> GetByIdAsync(int id, int tenantId);
        Task<UserLogin> GetByIdWithUserAsync(int id, int tenantId);
        Task<UserLogin> GetByProviderAsync(string loginProvider, string providerKey, int tenantId);
        Task<IEnumerable<UserLogin>> GetByUserIdAsync(int userId, int tenantId);
        Task<IEnumerable<UserLogin>> GetByLoginProviderAsync(string loginProvider, int tenantId);
        Task<UserLogin> GetUserLoginAsync(int userId, string loginProvider, int tenantId);
        Task<int> InsertAsync(UserLogin userLogin);
        Task<bool> UpdateAsync(UserLogin userLogin);
        Task<IEnumerable<UserLogin>> GetAllAsync(int tenantId);
        Task<PagedResult<UserLogin>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByUserAndProviderAsync(int userId, string loginProvider, int tenantId);
        Task<bool> ExistsAsync(string loginProvider, string providerKey, int tenantId);
        Task<User> FindUserByLoginAsync(string loginProvider, string providerKey, int tenantId);
        Task<IEnumerable<string>> GetLoginProvidersAsync(int tenantId);
        Task<int> GetUserLoginCountAsync(int userId, int tenantId);
    }
}
