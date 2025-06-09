using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories.RoleRepositories;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.RoleServices
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IRoleClaimRepository _roleClaimRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleClaimService(
            IRoleClaimRepository roleClaimRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleClaimRepository = roleClaimRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<RoleClaim>> GetRoleClaimByIdAsync(int roleClaimId)
        {
            try
            {
                var roleClaim = await _roleClaimRepository.GetByIdAsync(roleClaimId);
                if (roleClaim == null)
                {
                    return ServiceResult<RoleClaim>.FailureResult("Role claim not found");
                }

                return ServiceResult<RoleClaim>.SuccessResult(roleClaim);
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleClaim>.FailureResult($"Error retrieving role claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleClaim>>> GetByRoleIdAsync(int roleId)
        {
            try
            {
                var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(roleId);
                return ServiceResult<IEnumerable<RoleClaim>>.SuccessResult(roleClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RoleClaim>>.FailureResult($"Error retrieving role claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleClaim>>> GetByClaimTypeAsync(string claimType)
        {
            try
            {
                var roleClaims = await _roleClaimRepository.GetByClaimTypeAsync(claimType);
                return ServiceResult<IEnumerable<RoleClaim>>.SuccessResult(roleClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RoleClaim>>.FailureResult($"Error retrieving role claims by type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RoleClaim>> GetByRoleAndClaimAsync(int roleId, string claimType, string claimValue)
        {
            try
            {
                var roleClaim = await _roleClaimRepository.GetByRoleAndClaimAsync(roleId, claimType, claimValue);
                if (roleClaim == null)
                {
                    return ServiceResult<RoleClaim>.FailureResult("Role claim not found");
                }

                return ServiceResult<RoleClaim>.SuccessResult(roleClaim);
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleClaim>.FailureResult($"Error retrieving role claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleClaim>>> GetClaimsByRoleNameAsync(string roleName, int tenantId)
        {
            try
            {
                var roleClaims = await _roleClaimRepository.GetClaimsByRoleNameAsync(roleName, tenantId);
                return ServiceResult<IEnumerable<RoleClaim>>.SuccessResult(roleClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RoleClaim>>.FailureResult($"Error retrieving claims by role name: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateRoleClaimAsync(RoleClaim roleClaim)
        {
            try
            {
                // Validate role exists
                var role = await _roleRepository.GetByIdAsync(roleClaim.RoleId, 0); // Assuming single tenant for role validation
                if (role == null)
                {
                    return ServiceResult<int>.FailureResult("Role not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(roleClaim.ClaimType, roleClaim.ClaimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<int>.FailureResult(claimValidation.Message);
                }

                // Check if claim already exists for this role
                var existingClaim = await _roleClaimRepository.GetByRoleAndClaimAsync(roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);
                if (existingClaim != null)
                {
                    return ServiceResult<int>.FailureResult("This claim already exists for the role");
                }

                _unitOfWork.BeginTransaction();

                int roleClaimId = await _roleClaimRepository.InsertAsync(roleClaim);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(roleClaimId, "Role claim created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating role claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(RoleClaim roleClaim)
        {
            try
            {
                // Check if role claim exists
                var existingClaim = await _roleClaimRepository.GetByIdAsync(roleClaim.RoleClaimId);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("Role claim not found");
                }

                // Validate role exists
                var role = await _roleRepository.GetByIdAsync(roleClaim.RoleId, 0);
                if (role == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(roleClaim.ClaimType, roleClaim.ClaimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(claimValidation.Message);
                }

                // Check if another claim with same type and value exists for this role
                var duplicateClaim = await _roleClaimRepository.GetByRoleAndClaimAsync(roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);
                if (duplicateClaim != null && duplicateClaim.RoleClaimId != roleClaim.RoleClaimId)
                {
                    return ServiceResult<bool>.FailureResult("This claim already exists for the role");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _roleClaimRepository.UpdateAsync(roleClaim);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role claim updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating role claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int roleClaimId)
        {
            try
            {
                // Check if role claim exists
                var existingClaim = await _roleClaimRepository.GetByIdAsync(roleClaimId);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("Role claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _roleClaimRepository.DeleteAsync(roleClaimId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role claim deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting role claim: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByRoleIdAsync(int roleId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                bool result = await _roleClaimRepository.DeleteByRoleIdAsync(roleId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role claims deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting role claims: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddClaimToRoleAsync(int roleId, string claimType, string claimValue)
        {
            try
            {
                // Validate role exists
                var role = await _roleRepository.GetByIdAsync(roleId, 0);
                if (role == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Validate claim
                var claimValidation = await ValidateClaimAsync(claimType, claimValue);
                if (!claimValidation.Success)
                {
                    return ServiceResult<bool>.FailureResult(claimValidation.Message);
                }

                // Check if claim already exists
                bool claimExists = await _roleClaimRepository.ClaimExistsForRoleAsync(roleId, claimType, claimValue);
                if (claimExists)
                {
                    return ServiceResult<bool>.FailureResult("This claim already exists for the role");
                }

                var roleClaim = new RoleClaim
                {
                    RoleId = roleId,
                    ClaimType = claimType,
                    ClaimValue = claimValue
                };

                _unitOfWork.BeginTransaction();

                await _roleClaimRepository.InsertAsync(roleClaim);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claim added to role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding claim to role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveClaimFromRoleAsync(int roleId, string claimType, string claimValue)
        {
            try
            {
                var existingClaim = await _roleClaimRepository.GetByRoleAndClaimAsync(roleId, claimType, claimValue);
                if (existingClaim == null)
                {
                    return ServiceResult<bool>.FailureResult("Role claim not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _roleClaimRepository.DeleteAsync(existingClaim.RoleClaimId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Claim removed from role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing claim from role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ClaimExistsForRoleAsync(int roleId, string claimType, string claimValue)
        {
            try
            {
                bool exists = await _roleClaimRepository.ClaimExistsForRoleAsync(roleId, claimType, claimValue);
                return ServiceResult<bool>.SuccessResult(exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking claim existence: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetClaimTypesAsync()
        {
            try
            {
                var claimTypes = await _roleClaimRepository.GetClaimTypesAsync();
                return ServiceResult<IEnumerable<string>>.SuccessResult(claimTypes);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<string>>.FailureResult($"Error retrieving claim types: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleClaim>>> GetClaimsByTypeAsync(string claimType)
        {
            try
            {
                var roleClaims = await _roleClaimRepository.GetClaimsByTypeAsync(claimType);
                return ServiceResult<IEnumerable<RoleClaim>>.SuccessResult(roleClaims);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RoleClaim>>.FailureResult($"Error retrieving claims by type: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync()
        {
            return await _roleClaimRepository.GetAllAsync();
        }

        public async Task<ServiceResult<bool>> AddMultipleClaimsToRoleAsync(int roleId, List<(string ClaimType, string ClaimValue)> claims)
        {
            try
            {
                // Validate role exists
                var role = await _roleRepository.GetByIdAsync(roleId, 0);
                if (role == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
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
                    bool claimExists = await _roleClaimRepository.ClaimExistsForRoleAsync(roleId, claimType, claimValue);
                    if (!claimExists)
                    {
                        var roleClaim = new RoleClaim
                        {
                            RoleId = roleId,
                            ClaimType = claimType,
                            ClaimValue = claimValue
                        };

                        await _roleClaimRepository.InsertAsync(roleClaim);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claims added to role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding multiple claims to role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveMultipleClaimsFromRoleAsync(int roleId, List<(string ClaimType, string ClaimValue)> claims)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                foreach (var (claimType, claimValue) in claims)
                {
                    var existingClaim = await _roleClaimRepository.GetByRoleAndClaimAsync(roleId, claimType, claimValue);
                    if (existingClaim != null)
                    {
                        await _roleClaimRepository.DeleteAsync(existingClaim.RoleClaimId);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Claims removed from role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing multiple claims from role: {ex.Message}");
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

                // Validate common claim types format
                var validClaimTypes = new[]
                {
                    "permission",
                    "role",
                    "scope",
                    "feature",
                    "module",
                    "action",
                    "resource",
                    "department",
                    "location",
                    "level"
                };

                // Allow custom claim types but validate format
                if (!claimType.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-'))
                {
                    return ServiceResult<bool>.FailureResult("Claim type can only contain letters, digits, dots, underscores, and hyphens");
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
