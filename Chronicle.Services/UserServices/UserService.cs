using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories.UsersRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<User>> GetUserByIdAsync(int userId, int tenantId)
        {
            try
            {
                // Use the enhanced repository method that fetches user with all related data
                var user = await _userRepository.GetByIdWithRolesAsync(userId, tenantId);
                if (user == null)
                {
                    return ServiceResult<User>.FailureResult("User not found");
                }

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.FailureResult($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> GetByUserNameAsync(string userName, int tenantId)
        {
            try
            {
                var user = await _userRepository.GetByUserNameAsync(userName, tenantId);
                if (user == null)
                {
                    return ServiceResult<User>.FailureResult("User not found");
                }

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.FailureResult($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> GetByEmailAsync(string email, int tenantId)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email, tenantId);
                if (user == null)
                {
                    return ServiceResult<User>.FailureResult("User not found");
                }

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.FailureResult($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<User>> GetByEmployeeIdAsync(int employeeId, int tenantId)
        {
            try
            {
                var user = await _userRepository.GetByEmployeeIdAsync(employeeId, tenantId);
                if (user == null)
                {
                    return ServiceResult<User>.FailureResult("User not found");
                }

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.FailureResult($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetActiveUsersAsync(int tenantId)
        {
            try
            {
                var users = await _userRepository.GetActiveUsersAsync(tenantId);
                return ServiceResult<IEnumerable<User>>.SuccessResult(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.FailureResult($"Error retrieving active users: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetUsersByRoleAsync(string roleName, int tenantId)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(roleName, tenantId);
                return ServiceResult<IEnumerable<User>>.SuccessResult(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.FailureResult($"Error retrieving users by role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateUserAsync(User user, int tenantId)
        {
            try
            {
                // Set tenant ID
                user.TenantID = tenantId;

                // Check if username already exists within the tenant
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var existingUserByUserName = await _userRepository.GetByUserNameAsync(user.UserName, tenantId);
                    if (existingUserByUserName != null)
                    {
                        return ServiceResult<int>.FailureResult("User with this username already exists");
                    }
                }

                // Check if email already exists within the tenant
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var existingUserByEmail = await _userRepository.GetByEmailAsync(user.Email, tenantId);
                    if (existingUserByEmail != null)
                    {
                        return ServiceResult<int>.FailureResult("User with this email already exists");
                    }
                }

                // Check if employee ID already exists within the tenant
                if (user.EmployeeID > 0)
                {
                    var existingUserByEmployeeId = await _userRepository.GetByEmployeeIdAsync(user.EmployeeID, tenantId);
                    if (existingUserByEmployeeId != null)
                    {
                        return ServiceResult<int>.FailureResult("User with this employee ID already exists");
                    }
                }

                // Set default values
                user.CreatedDate = DateTime.UtcNow;
                user.LastModifiedDate = DateTime.UtcNow;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();

                // Normalize username and email
                user.NormalizedUserName = user.UserName?.ToUpperInvariant();
                user.NormalizedEmail = user.Email?.ToUpperInvariant();

                _unitOfWork.BeginTransaction();

                int userId = await _userRepository.InsertAsync(user);

                // Insert user roles if any
                if (user.UserRoles != null && user.UserRoles.Any())
                {
                    foreach (UserRole userRole in user.UserRoles)
                    {
                        userRole.UserID = userId;
                        userRole.TenantID = user.TenantID;

                        await _userRoleRepository.InsertAsync(userRole);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(userId, "User created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(User user, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                user.TenantID = tenantId;

                // Check if user exists within the tenant
                var existingUser = await _userRepository.GetByIdAsync(user.UserId, tenantId);
                if (existingUser == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Check if username is unique within the tenant
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var userByUserName = await _userRepository.GetByUserNameAsync(user.UserName, tenantId);
                    if (userByUserName != null && userByUserName.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Username already exists");
                    }
                }

                // Check if email is unique within the tenant
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var userByEmail = await _userRepository.GetByEmailAsync(user.Email, tenantId);
                    if (userByEmail != null && userByEmail.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Email already exists");
                    }
                }

                // Check if employee ID is unique within the tenant
                if (user.EmployeeID > 0)
                {
                    var userByEmployeeId = await _userRepository.GetByEmployeeIdAsync(user.EmployeeID, tenantId);
                    if (userByEmployeeId != null && userByEmployeeId.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Employee ID already exists");
                    }
                }

                // Normalize username and email
                user.NormalizedUserName = user.UserName?.ToUpperInvariant();
                user.NormalizedEmail = user.Email?.ToUpperInvariant();
                user.LastModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _userRepository.UpdateAsync(user);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateWithRolesAsync(User user, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                user.TenantID = tenantId;

                // Check if user exists within the tenant
                var existingUser = await _userRepository.GetByIdAsync(user.UserId, tenantId);
                if (existingUser == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                // Check if username is unique within the tenant
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var userByUserName = await _userRepository.GetByUserNameAsync(user.UserName, tenantId);
                    if (userByUserName != null && userByUserName.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Username already exists");
                    }
                }

                // Check if email is unique within the tenant
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var userByEmail = await _userRepository.GetByEmailAsync(user.Email, tenantId);
                    if (userByEmail != null && userByEmail.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Email already exists");
                    }
                }

                // Check if employee ID is unique within the tenant
                if (user.EmployeeID > 0)
                {
                    var userByEmployeeId = await _userRepository.GetByEmployeeIdAsync(user.EmployeeID, tenantId);
                    if (userByEmployeeId != null && userByEmployeeId.UserId != user.UserId)
                    {
                        return ServiceResult<bool>.FailureResult("Employee ID already exists");
                    }
                }

                // Normalize username and email
                user.NormalizedUserName = user.UserName?.ToUpperInvariant();
                user.NormalizedEmail = user.Email?.ToUpperInvariant();
                user.LastModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                // Update the user
                bool userUpdateResult = await _userRepository.UpdateAsync(user);
                if (!userUpdateResult)
                {
                    throw new Exception("Failed to update user");
                }

                // Handle user roles update
                await UpdateUserRolesInTransactionAsync(user.UserId, user.UserRoles, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "User and roles updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating user with roles: {ex.Message}");
            }
        }

        private async Task UpdateUserRolesInTransactionAsync(int userId, ICollection<UserRole> newRoles, int tenantId)
        {
            try
            {
                // Get existing roles for this user
                var existingRoles = await _userRoleRepository.GetByUserIdAsync(userId, tenantId);
                var existingRolesList = existingRoles.ToList();

                if (newRoles == null)
                {
                    newRoles = new List<UserRole>();
                }

                // Find roles to delete (exist in DB but not in new list)
                var rolesToDelete = existingRolesList.Where(existing =>
                    !newRoles.Any(newRole => newRole.RoleID == existing.RoleID)).ToList();

                // Find roles to add (in new list but don't exist in DB)
                var rolesToAdd = newRoles.Where(newRole =>
                    !existingRolesList.Any(existing => existing.RoleID == newRole.RoleID)).ToList();

                // Delete removed roles
                foreach (var roleToDelete in rolesToDelete)
                {
                    await _userRoleRepository.DeleteAsync(roleToDelete.UserRoleID, tenantId);
                }

                // Add new roles
                foreach (var newRole in rolesToAdd)
                {
                    newRole.UserID = userId;
                    newRole.TenantID = tenantId;

                    await _userRoleRepository.InsertAsync(newRole);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user roles: {ex.Message}", ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int userId, int tenantId)
        {
            try
            {
                // Check if user exists within the tenant
                var existingUser = await _userRepository.GetByIdAsync(userId, tenantId);
                if (existingUser == null)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                _unitOfWork.BeginTransaction();

                // Delete associated user roles first
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId, tenantId);
                foreach (var userRole in userRoles)
                {
                    await _userRoleRepository.DeleteAsync(userRole.UserRoleID, tenantId);
                }

                // Delete the user
                bool result = await _userRepository.DeleteAsync(userId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "User deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int tenantId)
        {
            return await _userRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<User>>> GetPagedUsersAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var users = await _userRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<User>>.SuccessResult(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<User>>.FailureResult($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateLastLoginAsync(int userId, int tenantId)
        {
            try
            {
                bool result = await _userRepository.UpdateLastLoginAsync(userId, tenantId);
                return ServiceResult<bool>.SuccessResult(result, "Last login updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error updating last login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IncrementAccessFailedCountAsync(int userId, int tenantId)
        {
            try
            {
                bool result = await _userRepository.IncrementAccessFailedCountAsync(userId, tenantId);
                return ServiceResult<bool>.SuccessResult(result, "Access failed count incremented successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error incrementing access failed count: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ResetAccessFailedCountAsync(int userId, int tenantId)
        {
            try
            {
                bool result = await _userRepository.ResetAccessFailedCountAsync(userId, tenantId);
                return ServiceResult<bool>.SuccessResult(result, "Access failed count reset successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error resetting access failed count: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> SetLockoutAsync(int userId, DateTimeOffset? lockoutEnd, int tenantId)
        {
            try
            {
                bool result = await _userRepository.SetLockoutAsync(userId, lockoutEnd, tenantId);
                return ServiceResult<bool>.SuccessResult(result, "Lockout status updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error updating lockout status: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidatePasswordAsync(string password)
        {
            try
            {
                // Basic password validation rules
                if (string.IsNullOrEmpty(password))
                {
                    return ServiceResult<bool>.FailureResult("Password cannot be empty");
                }

                if (password.Length < 6)
                {
                    return ServiceResult<bool>.FailureResult("Password must be at least 6 characters long");
                }

                // Add more validation rules as needed
                // Example: check for uppercase, lowercase, digits, special characters

                return ServiceResult<bool>.SuccessResult(true, "Password is valid");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error validating password: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, int tenantId)
        {
            try
            {
                // Get the user
                var userResult = await GetUserByIdAsync(userId, tenantId);
                if (!userResult.Success)
                {
                    return ServiceResult<bool>.FailureResult("User not found");
                }

                var user = userResult.Data;

                // Verify current password (you would implement password hashing/verification here)
                // This is a simplified example - in real implementation, use proper password hashing
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return ServiceResult<bool>.FailureResult("Current password is incorrect");
                }

                // Validate new password
                var passwordValidation = await ValidatePasswordAsync(newPassword);
                if (!passwordValidation.Success)
                {
                    return passwordValidation;
                }

                // Hash the new password (simplified - use proper hashing in real implementation)
                user.PasswordHash = HashPassword(newPassword);
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.LastModifiedDate = DateTime.UtcNow;

                var updateResult = await UpdateAsync(user, tenantId);
                if (updateResult.Success)
                {
                    return ServiceResult<bool>.SuccessResult(true, "Password changed successfully");
                }

                return ServiceResult<bool>.FailureResult("Failed to update password");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error changing password: {ex.Message}");
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            // Simplified password verification - implement proper password hashing/verification
            // This is just for example purposes
            return HashPassword(password) == hash;
        }

        private string HashPassword(string password)
        {
            // Simplified password hashing - implement proper password hashing
            // Use BCrypt, Argon2, or similar in real implementation
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
