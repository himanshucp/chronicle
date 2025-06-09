using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.UsersRepositories
{
    public interface IUserClaimRepository : IRepository<UserClaim, int>
    {
        Task<UserClaim> GetByIdAsync(int id, int tenantId);
        Task<UserClaim> GetByIdWithUserAsync(int id, int tenantId);
        Task<IEnumerable<UserClaim>> GetByUserIdAsync(int userId, int tenantId);
        Task<IEnumerable<UserClaim>> GetByClaimTypeAsync(string claimType, int tenantId);
        Task<UserClaim> GetByUserAndClaimAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<IEnumerable<UserClaim>> GetActiveUserClaimsAsync(int tenantId);
        Task<int> InsertAsync(UserClaim userClaim);
        Task<bool> UpdateAsync(UserClaim userClaim);
        Task<IEnumerable<UserClaim>> GetAllAsync(int tenantId);
        Task<PagedResult<UserClaim>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> DeleteByUserIdAsync(int userId, int tenantId);
        Task<bool> ClaimExistsForUserAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<IEnumerable<string>> GetClaimTypesAsync(int tenantId);
        Task<IEnumerable<UserClaim>> GetClaimsByTypeAsync(string claimType, int tenantId);
        Task<bool> DeactivateUserClaimAsync(int userClaimId, int tenantId);
        Task<bool> ActivateUserClaimAsync(int userClaimId, int tenantId);
        Task<int> GetUserClaimCountAsync(int userId, int tenantId);
    }
}
