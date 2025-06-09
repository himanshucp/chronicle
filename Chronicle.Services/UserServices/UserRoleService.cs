using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories.UsersRepositories;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<UserRole>> GetUserRoleByIdAsync(int userRoleId, int tenantId)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByIdWithDetailsAsync(userRoleId, tenantId);
                if (userRole == null)
                {
                    return ServiceResult<UserRole>.FailureResult("User role not found");
                }

                return ServiceResult<UserRole>.SuccessResult(userRole);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserRole>.FailureResult($"Error retrieving user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserRole>> GetByUserAndRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, tenantId);
                if (userRole == null)
                {
                    return ServiceResult<UserRole>.FailureResult("User role assignment not found");
                }

                return ServiceResult<UserRole>.SuccessResult(userRole);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserRole>.FailureResult($"Error retrieving user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRole>>> GetByUserIdAsync(int userId, int tenantId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId, tenantId);
                return ServiceResult<IEnumerable<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserRole>>.FailureResult($"Error retrieving user roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRole>>> GetByRoleIdAsync(int roleId, int tenantId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetByRoleIdAsync(roleId, tenantId);
                return ServiceResult<IEnumerable<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserRole>>.FailureResult($"Error retrieving user roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRole>>> GetActiveUserRolesAsync(int tenantId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetActiveUserRolesAsync(tenantId);
                return ServiceResult<IEnumerable<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserRole>>.FailureResult($"Error retrieving active user roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRole>>> GetUserRolesByUserAsync(int userId, int tenantId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetUserRolesByUserAsync(userId, tenantId);
                return ServiceResult<IEnumerable<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserRole>>.FailureResult($"Error retrieving user roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<UserRole>>> GetUsersInRoleAsync(int roleId, int tenantId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetUsersInRoleAsync(roleId, tenantId);
                return ServiceResult<IEnumerable<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<UserRole>>.FailureResult($"Error retrieving users in role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateUserRoleAsync(UserRole userRole, int tenantId)
        {
            try
            {
                // Set tenant ID
                userRole.TenantID = tenantId;

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userRole.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<int>.FailureResult("User not found");
                }

                // Validate that role exists
                var role = await _roleRepository.GetByIdAsync(userRole.RoleID, tenantId);
                if (role == null)
                {
                    return ServiceResult<int>.FailureResult("Role not found");
                }

                // Check if user is already assigned to this role
                var existingUserRole = await _userRoleRepository.GetByUserAndRoleAsync(userRole.UserID, userRole.RoleID, tenantId);
                if (existingUserRole != null)
                {
                    return ServiceResult<int>.FailureResult("User is already assigned to this role");
                }

                // Set default values
                userRole.CreatedDate = DateTime.UtcNow;
                userRole.ModifiedDate = DateTime.UtcNow;
                userRole.IsActive = true;

                _unitOfWork.BeginTransaction();

                int userRoleId = await _userRoleRepository.InsertAsync(userRole);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(userRoleId, "User role created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UserRole userRole, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                userRole.TenantID = tenantId;

                // Check if user role exists within the tenant
                var existingUserRole = await _userRoleRepository.GetByIdAsync(userRole.UserRoleID, tenantId);
                if (existingUserRole == null)
                {
                    return ServiceResult<bool>.FailureResult("User role not found");
                }

                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userRole.UserID, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate that role exists
                var role = await _roleRepository.GetByIdAsync(userRole.RoleID, tenantId);
                if (role == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Check if another user role record exists with the same user and role combination
                var duplicateUserRole = await _userRoleRepository.GetByUserAndRoleAsync(userRole.UserID, userRole.RoleID, tenantId);
                if (duplicateUserRole != null && duplicateUserRole.UserRoleID != userRole.UserRoleID)
                {
                    return ServiceResult<bool>.FailureResult("User is already assigned to this role");
                }

                userRole.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _userRoleRepository.UpdateAsync(userRole);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User role updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int userRoleId, int tenantId)
        {
            try
            {
                // Check if user role exists within the tenant
                var existingUserRole = await _userRoleRepository.GetByIdAsync(userRoleId, tenantId);
                if (existingUserRole == null)
                {
                    return ServiceResult<bool>.FailureResult("User role not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userRoleRepository.DeleteAsync(userRoleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User role deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddUserToRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                // Validate that user exists
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Validate that role exists
                var role = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (role == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Check if user is already assigned to this role
                var existingUserRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, tenantId);
                if (existingUserRole != null)
                {
                    return ServiceResult<bool>.FailureResult("User is already assigned to this role");
                }

                var userRole = new UserRole
                {
                    UserID = userId,
                    RoleID = roleId,
                    TenantID = tenantId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _unitOfWork.BeginTransaction();

                await _userRoleRepository.InsertAsync(userRole);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User added to role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding user to role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                // Check if user role assignment exists
                var userRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, tenantId);
                if (userRole == null)
                {
                    return ServiceResult<bool>.FailureResult("User role assignment not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userRoleRepository.RemoveUserFromRoleAsync(userId, roleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User removed from role successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing user from role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UserIsInRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                bool isInRole = await _userRoleRepository.UserIsInRoleAsync(userId, roleId, tenantId);
                return ServiceResult<bool>.SuccessResult(isInRole);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateUserRoleAsync(int userRoleId, int tenantId)
        {
            try
            {
                // Check if user role exists
                var userRole = await _userRoleRepository.GetByIdAsync(userRoleId, tenantId);
                if (userRole == null)
                {
                    return ServiceResult<bool>.FailureResult("User role not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userRoleRepository.DeactivateUserRoleAsync(userRoleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User role deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateUserRoleAsync(int userRoleId, int tenantId)
        {
            try
            {
                // Check if user role exists
                var userRole = await _userRoleRepository.GetByIdAsync(userRoleId, tenantId);
                if (userRole == null)
                {
                    return ServiceResult<bool>.FailureResult("User role not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _userRoleRepository.ActivateUserRoleAsync(userRoleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User role activated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error activating user role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddUserToMultipleRolesAsync(int userId, List<int> roleIds, int tenantId)
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

                foreach (int roleId in roleIds)
                {
                    // Validate that role exists
                    var role = await _roleRepository.GetByIdAsync(roleId, tenantId);
                    if (role == null)
                    {
                        _unitOfWork.Rollback();
                        return ServiceResult<bool>.FailureResult($"Role with ID {roleId} not found");
                    }

                    // Check if user is already assigned to this role
                    var existingUserRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, tenantId);
                    if (existingUserRole == null)
                    {
                        var userRole = new UserRole
                        {
                            UserID = userId,
                            RoleID = roleId,
                            TenantID = tenantId,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            IsActive = true
                        };

                        await _userRoleRepository.InsertAsync(userRole);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User added to multiple roles successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error adding user to multiple roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveUserFromMultipleRolesAsync(int userId, List<int> roleIds, int tenantId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                foreach (int roleId in roleIds)
                {
                    await _userRoleRepository.RemoveUserFromRoleAsync(userId, roleId, tenantId);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User removed from multiple roles successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error removing user from multiple roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateUserRolesAsync(int userId, List<int> newRoleIds, int tenantId)
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

                // Get current user roles
                var currentUserRoles = await _userRoleRepository.GetByUserIdAsync(userId, tenantId);
                var currentRoleIds = currentUserRoles.Select(ur => ur.RoleID).ToList();

                // Determine roles to add and remove
                var rolesToAdd = newRoleIds.Except(currentRoleIds).ToList();
                var rolesToRemove = currentRoleIds.Except(newRoleIds).ToList();

                // Remove roles that are no longer needed
                foreach (int roleId in rolesToRemove)
                {
                    await _userRoleRepository.RemoveUserFromRoleAsync(userId, roleId, tenantId);
                }

                // Add new roles
                foreach (int roleId in rolesToAdd)
                {
                    // Validate that role exists
                    var role = await _roleRepository.GetByIdAsync(roleId, tenantId);
                    if (role == null)
                    {
                        _unitOfWork.Rollback();
                        return ServiceResult<bool>.FailureResult($"Role with ID {roleId} not found");
                    }

                    var userRole = new UserRole
                    {
                        UserID = userId,
                        RoleID = roleId,
                        TenantID = tenantId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _userRoleRepository.InsertAsync(userRole);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User roles updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user roles: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int tenantId)
        {
            return await _userRoleRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<UserRole>>> GetPagedUserRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<UserRole>>.SuccessResult(userRoles);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<UserRole>>.FailureResult($"Error retrieving user roles: {ex.Message}");
            }
        }
    }
}
