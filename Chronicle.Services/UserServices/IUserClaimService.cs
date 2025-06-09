using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public interface IUserClaimService
    {
        Task<ServiceResult<UserClaim>> GetUserClaimByIdAsync(int userClaimId, int tenantId);
        Task<ServiceResult<IEnumerable<UserClaim>>> GetByUserIdAsync(int userId, int tenantId);
        Task<ServiceResult<IEnumerable<UserClaim>>> GetByClaimTypeAsync(string claimType, int tenantId);
        Task<ServiceResult<UserClaim>> GetByUserAndClaimAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<ServiceResult<IEnumerable<UserClaim>>> GetActiveUserClaimsAsync(int tenantId);
        Task<ServiceResult<int>> CreateUserClaimAsync(UserClaim userClaim, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(UserClaim userClaim, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int userClaimId, int tenantId);
        Task<ServiceResult<bool>> DeleteByUserIdAsync(int userId, int tenantId);
        Task<ServiceResult<bool>> AddClaimToUserAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<ServiceResult<bool>> RemoveClaimFromUserAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<ServiceResult<bool>> ClaimExistsForUserAsync(int userId, string claimType, string claimValue, int tenantId);
        Task<ServiceResult<IEnumerable<string>>> GetClaimTypesAsync(int tenantId);
        Task<ServiceResult<IEnumerable<UserClaim>>> GetClaimsByTypeAsync(string claimType, int tenantId);
        Task<ServiceResult<bool>> DeactivateUserClaimAsync(int userClaimId, int tenantId);
        Task<ServiceResult<bool>> ActivateUserClaimAsync(int userClaimId, int tenantId);
        Task<ServiceResult<int>> GetUserClaimCountAsync(int userId, int tenantId);
        Task<IEnumerable<UserClaim>> GetUserClaimsAsync(int tenantId);
        Task<ServiceResult<PagedResult<UserClaim>>> GetPagedUserClaimsAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<bool>> AddMultipleClaimsToUserAsync(int userId, List<(string ClaimType, string ClaimValue)> claims, int tenantId);
        Task<ServiceResult<bool>> RemoveMultipleClaimsFromUserAsync(int userId, List<(string ClaimType, string ClaimValue)> claims, int tenantId);
        Task<ServiceResult<bool>> UpdateUserClaimsAsync(int userId, List<(string ClaimType, string ClaimValue)> newClaims, int tenantId);
        Task<ServiceResult<bool>> ValidateClaimAsync(string claimType, string claimValue);
    }
}
