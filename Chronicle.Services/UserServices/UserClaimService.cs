using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories.UsersRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public class UserClaimService : IUserClaimService
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserClaimService(
            IUserClaimRepository userClaimRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userClaimRepository = userClaimRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<UserClaim>> GetUserClaimByIdAsync(int userClaimId, int tenantId)
        {
            try
            {
                var userClaim = await _userClaimRepository.GetByIdWithUserAsync(userClaimId, tenantId);
                if (userClaim == null)
                {
                    return ServiceResult<UserClaim>.FailureResult("User claim not found");
                }

                return ServiceResult<UserClaim>.SuccessResult(userClaim);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserClaim>.FailureResult($"Error retrieving user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserClaim>>> GetByUserIdAsync(int userId, int tenantId)
        {
            try
            {
                var userClaims = await _userClaimRepository.GetByUserIdAsync(userId, tenantId);
                return ServiceResult<IEnumerable<UserClaim>>.SuccessResult(userClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserClaim>>.FailureResult($"Error retrieving user claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserClaim>>> GetByClaimTypeAsync(string claimType, int tenantId)
        {
            try
            {
                var userClaims = await _userClaimRepository.GetByClaimTypeAsync(claimType, tenantId);
                return ServiceResult<IEnumerable<UserClaim>>.SuccessResult(userClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserClaim>>.FailureResult($"Error retrieving user claims by type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserClaim>> GetByUserAndClaimAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            try
            {
                var userClaim = await _userClaimRepository.GetByUserAndClaimAsync(userId, claimType, claimValue, tenantId);
                if (userClaim == null)
                {
                    return ServiceResult<UserClaim>.FailureResult("User claim not found");
                }

                return ServiceResult<UserClaim>.SuccessResult(userClaim);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserClaim>.FailureResult($"Error retrieving user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserClaim>>> GetActiveUserClaimsAsync(int tenantId)
        {
            try
            {
                var userClaims = await _userClaimRepository.GetActiveUserClaimsAsync(tenantId);
                return ServiceResult<IEnumerable<UserClaim>>.SuccessResult(userClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserClaim>>.FailureResult($"Error retrieving active user claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateUserClaimAsync(UserClaim userClaim, int tenantId)
        {
            try
            {
                // Set tenant ID
                userClaim.TenantID = tenantId;

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userClaim.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<int>.FailureResult("User not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(userClaim.ClaimType, userClaim.ClaimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<int>.FailureResult(claimValidation.Message);
                }

                // Check if claim already exists for this user
                var existingClaim = await _userClaimRepository.GetByUserAndClaimAsync(userClaim.UserID, userClaim.ClaimType, userClaim.ClaimValue, tenantId);
                if (existingClaim != null)
                {
                    return ServiceResult<int>.FailureResult("This claim already exists for the user");
                }

                // Set default values
                userClaim.CreatedDate = DateTime.UtcNow;
                userClaim.ModifiedDate = DateTime.UtcNow;
                userClaim.IsActive = true;

                _unitOfWork.BeginTransaction();

                int userClaimId = await _userClaimRepository.InsertAsync(userClaim);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(userClaimId, "User claim created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UserClaim userClaim, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                userClaim.TenantID = tenantId;

                // Check if user claim exists
                var existingClaim = await _userClaimRepository.GetByIdAsync(userClaim.UserClaimID, tenantId);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("User claim not found");
                }

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userClaim.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(userClaim.ClaimType, userClaim.ClaimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(claimValidation.Message);
                }

                // Check if another claim with same type and value exists for this user
                var duplicateClaim = await _userClaimRepository.GetByUserAndClaimAsync(userClaim.UserID, userClaim.ClaimType, userClaim.ClaimValue, tenantId);
                if (duplicateClaim != null && duplicateClaim.UserClaimID != userClaim.UserClaimID)
                {
                    return ServiceResult<bool>.FailureResult("This claim already exists for the user");
                }

                userClaim.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.UpdateAsync(userClaim);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User claim updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int userClaimId, int tenantId)
        {
            try
            {
                // Check if user claim exists
                var existingClaim = await _userClaimRepository.GetByIdAsync(userClaimId, tenantId);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("User claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.DeleteAsync(userClaimId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User claim deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByUserIdAsync(int userId, int tenantId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.DeleteByUserIdAsync(userId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User claims deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting user claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddClaimToUserAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            try
            {
                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(claimType, claimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(claimValidation.Message);
                }

                // Check if claim already exists
                bool claimExists = await _userClaimRepository.ClaimExistsForUserAsync(userId, claimType, claimValue, tenantId);
                if (claimExists)
                {
                    return ServiceResult<bool>.FailureResult("This claim already exists for the user");
                }

                var userClaim = new UserClaim
                {
                    UserID = userId,
                    TenantID = tenantId,
                    ClaimType = claimType,
                    ClaimValue = claimValue,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _unitOfWork.BeginTransaction();

                await _userClaimRepository.InsertAsync(userClaim);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claim added to user successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding claim to user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveClaimFromUserAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            try
            {
                var existingClaim = await _userClaimRepository.GetByUserAndClaimAsync(userId, claimType, claimValue, tenantId);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("User claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.DeleteAsync(existingClaim.UserClaimID, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Claim removed from user successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing claim from user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ClaimExistsForUserAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            try
            {
                bool exists = await _userClaimRepository.ClaimExistsForUserAsync(userId, claimType, claimValue, tenantId);
                return ServiceResult<bool>.SuccessResult(exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking claim existence: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetClaimTypesAsync(int tenantId)
        {
            try
            {
                var claimTypes = await _userClaimRepository.GetClaimTypesAsync(tenantId);
                return ServiceResult<IEnumerable<string>>.SuccessResult(claimTypes);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<string>>.FailureResult($"Error retrieving claim types: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserClaim>>> GetClaimsByTypeAsync(string claimType, int tenantId)
        {
            try
            {
                var userClaims = await _userClaimRepository.GetClaimsByTypeAsync(claimType, tenantId);
                return ServiceResult<IEnumerable<UserClaim>>.SuccessResult(userClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserClaim>>.FailureResult($"Error retrieving claims by type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateUserClaimAsync(int userClaimId, int tenantId)
        {
            try
            {
                // Check if user claim exists
                var userClaim = await _userClaimRepository.GetByIdAsync(userClaimId, tenantId);
                if (userClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("User claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.DeactivateUserClaimAsync(userClaimId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User claim deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateUserClaimAsync(int userClaimId, int tenantId)
        {
            try
            {
                // Check if user claim exists
                var userClaim = await _userClaimRepository.GetByIdAsync(userClaimId, tenantId);
                if (userClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("User claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userClaimRepository.ActivateUserClaimAsync(userClaimId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User claim activated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error activating user claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetUserClaimCountAsync(int userId, int tenantId)
        {
            try
            {
                int count = await _userClaimRepository.GetUserClaimCountAsync(userId, tenantId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error getting user claim count: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserClaim>> GetUserClaimsAsync(int tenantId)
        {
            return await _userClaimRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<UserClaim>>> GetPagedUserClaimsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var userClaims = await _userClaimRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<UserClaim>>.SuccessResult(userClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<UserClaim>>.FailureResult($"Error retrieving user claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddMultipleClaimsToUserAsync(int userId, List<(string ClaimType, string ClaimValue)> claims, int tenantId)
        {
            try
            {
                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                _unitOfWork.BeginTransaction();

                foreach (var (claimType, claimValue) in claims)
                {
                    // Validate claim
                    var claimValidation = await ValidateClaimAsync(claimType, claimValue);
                    if (!claimValidation.Success)
                    {
                        _unitOfWork.Rollback();
                        return ServiceResult<bool>.FailureResult($"Invalid claim {claimType}: {claimValidation.Message}");
                    }

                    // Check if claim already exists
                    bool claimExists = await _userClaimRepository.ClaimExistsForUserAsync(userId, claimType, claimValue, tenantId);
                    if (!claimExists)
                    {
                        var userClaim = new UserClaim
                        {
                            UserID = userId,
                            TenantID = tenantId,
                            ClaimType = claimType,
                            ClaimValue = claimValue,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            IsActive = true
                        };

                        await _userClaimRepository.InsertAsync(userClaim);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claims added to user successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding multiple claims to user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveMultipleClaimsFromUserAsync(int userId, List<(string ClaimType, string ClaimValue)> claims, int tenantId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                foreach (var (claimType, claimValue) in claims)
                {
                    var existingClaim = await _userClaimRepository.GetByUserAndClaimAsync(userId, claimType, claimValue, tenantId);
                    if (existingClaim != null)
                    {
                        await _userClaimRepository.DeleteAsync(existingClaim.UserClaimID, tenantId);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claims removed from user successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing multiple claims from user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateUserClaimsAsync(int userId, List<(string ClaimType, string ClaimValue)> newClaims, int tenantId)
        {
            try
            {
                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                _unitOfWork.BeginTransaction();

                // Get current user claims
                var currentUserClaims = await _userClaimRepository.GetByUserIdAsync(userId, tenantId);
                var currentClaims = currentUserClaims.Select(uc => (uc.ClaimType, uc.ClaimValue)).ToList();

                // Determine claims to add and remove
                var claimsToAdd = newClaims.Except(currentClaims).ToList();
                var claimsToRemove = currentClaims.Except(newClaims).ToList();

                // Remove claims that are no longer needed
                foreach (var (claimType, claimValue) in claimsToRemove)
                {
                    var claimToRemove = currentUserClaims.First(uc => uc.ClaimType == claimType && uc.ClaimValue == claimValue);
                    await _userClaimRepository.DeleteAsync(claimToRemove.UserClaimID, tenantId);
                }

                // Add new claims
                foreach (var (claimType, claimValue) in claimsToAdd)
                {
                    // Validate claim
                    var claimValidation = await ValidateClaimAsync(claimType, claimValue);
                    if (!claimValidation.Success)
                    {
                        _unitOfWork.Rollback();
                        return ServiceResult<bool>.FailureResult($"Invalid claim {claimType}: {claimValidation.Message}");
                    }

                    var userClaim = new UserClaim
                    {
                        UserID = userId,
                        TenantID = tenantId,
                        ClaimType = claimType,
                        ClaimValue = claimValue,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _userClaimRepository.InsertAsync(userClaim);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User claims updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidateClaimAsync(string claimType, string claimValue)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(claimType))
                {
                    return ServiceResult<bool>.FailureResult("Claim type cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(claimValue))
                {
                    return ServiceResult<bool>.FailureResult("Claim value cannot be empty");
                }

                if (claimType.Length > 255)
                {
                    return ServiceResult<bool>.FailureResult("Claim type cannot exceed 255 characters");
                }

                if (claimValue.Length > 255)
                {
                    return ServiceResult<bool>.FailureResult("Claim value cannot exceed 255 characters");
                }

                // Check for invalid characters
                if (claimType.Contains("<") || claimType.Contains(">") || claimType.Contains("&"))
                {
                    return ServiceResult<bool>.FailureResult("Claim type contains invalid characters");
                }

                return ServiceResult<bool>.SuccessResult(true, "Claim is valid");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error validating claim: {ex.Message}");
            }
        }
    }
}
