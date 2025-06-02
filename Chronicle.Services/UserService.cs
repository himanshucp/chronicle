using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    /// <summary>
    /// Implementation of user management service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> GetUserByIdAsync(int id, int tenantId)
        {
            return await _userRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(int tenantId)
        {
            return await _userRepository.GetAllAsync(tenantId);
        }

        public async Task<User> GetUserByUsernameAsync(string username, int tenantId)
        {
            return await _userRepository.GetByUsernameAsync(username, tenantId);
        }

        public async Task<User> GetUserByEmailAsync(string email, int tenantId)
        {
            return await _userRepository.GetByEmailAsync(email, tenantId);
        }

        public async Task<UserWithRoles> GetUserWithRolesAsync(int userId, int tenantId)
        {
            return await _userRepository.GetUserWithRolesAsync(userId, tenantId);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId, int tenantId)
        {
            return await _userRepository.GetUsersByRoleAsync(roleId, tenantId);
        }

        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure creation date is set
                if (user.CreatedDate == default)
                {
                    user.CreatedDate = DateTime.UtcNow;
                }

                int userId = await _userRepository.InsertAsync(user);
                await _unitOfWork.CommitAsync();
                return userId;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure modified date is updated
                user.LastModifiedDate = DateTime.UtcNow;

                bool result = await _userRepository.UpdateAsync(user);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                bool result = await _userRepository.DeleteAsync(id, tenantId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AddUserToRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                bool result = await _userRepository.AddUserToRoleAsync(userId, roleId, tenantId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                bool result = await _userRepository.RemoveUserFromRoleAsync(userId, roleId, tenantId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                bool result = await _userRepository.UpdateUserRolesAsync(userId, roleIds, tenantId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<User>> GetPagedUsersAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            return await _userRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
        }
    }

}
