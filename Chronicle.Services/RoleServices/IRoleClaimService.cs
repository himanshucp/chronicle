using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.RoleServices
{
    public interface IRoleClaimService
    {
        Task<ServiceResult<RoleClaim>> GetRoleClaimByIdAsync(int roleClaimId);
        Task<ServiceResult<IEnumerable<RoleClaim>>> GetByRoleIdAsync(int roleId);
        Task<ServiceResult<IEnumerable<RoleClaim>>> GetByClaimTypeAsync(string claimType);
        Task<ServiceResult<RoleClaim>> GetByRoleAndClaimAsync(int roleId, string claimType, string claimValue);
        Task<ServiceResult<IEnumerable<RoleClaim>>> GetClaimsByRoleNameAsync(string roleName, int tenantId);
        Task<ServiceResult<int>> CreateRoleClaimAsync(RoleClaim roleClaim);
        Task<ServiceResult<bool>> UpdateAsync(RoleClaim roleClaim);
        Task<ServiceResult<bool>> DeleteAsync(int roleClaimId);
        Task<ServiceResult<bool>> DeleteByRoleIdAsync(int roleId);
        Task<ServiceResult<bool>> AddClaimToRoleAsync(int roleId, string claimType, string claimValue);
        Task<ServiceResult<bool>> RemoveClaimFromRoleAsync(int roleId, string claimType, string claimValue);
        Task<ServiceResult<bool>> ClaimExistsForRoleAsync(int roleId, string claimType, string claimValue);
        Task<ServiceResult<IEnumerable<string>>> GetClaimTypesAsync();
        Task<ServiceResult<IEnumerable<RoleClaim>>> GetClaimsByTypeAsync(string claimType);
        Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync();
        Task<ServiceResult<bool>> AddMultipleClaimsToRoleAsync(int roleId, List<(string ClaimType, string ClaimValue)> claims);
        Task<ServiceResult<bool>> RemoveMultipleClaimsFromRoleAsync(int roleId, List<(string ClaimType, string ClaimValue)> claims);
        Task<ServiceResult<bool>> ValidateClaimAsync(string claimType, string claimValue);
    }
}
