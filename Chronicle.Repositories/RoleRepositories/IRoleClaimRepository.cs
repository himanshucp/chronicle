using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.RoleRepositories
{
    public interface IRoleClaimRepository : IRepository<RoleClaim, int>
    {
        Task<RoleClaim> GetByIdAsync(int id);
        Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<RoleClaim>> GetByClaimTypeAsync(string claimType);
        Task<RoleClaim> GetByRoleAndClaimAsync(int roleId, string claimType, string claimValue);
        Task<IEnumerable<RoleClaim>> GetClaimsByRoleNameAsync(string roleName, int tenantId);
        Task<int> InsertAsync(RoleClaim roleClaim);
        Task<bool> UpdateAsync(RoleClaim roleClaim);
        Task<IEnumerable<RoleClaim>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByRoleIdAsync(int roleId);
        Task<bool> ClaimExistsForRoleAsync(int roleId, string claimType, string claimValue);
        Task<IEnumerable<string>> GetClaimTypesAsync();
        Task<IEnumerable<RoleClaim>> GetClaimsByTypeAsync(string claimType);
    }
}
