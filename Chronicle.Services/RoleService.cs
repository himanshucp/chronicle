using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    /// <summary>
    /// Implementation of role management service
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Role> GetRoleByIdAsync(int id, int tenantId)
        {
            return await _roleRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(int tenantId)
        {
            return await _roleRepository.GetAllAsync(tenantId);
        }

        public async Task<Role> GetRoleByNameAsync(string name, int tenantId)
        {
            return await _roleRepository.GetByNameAsync(name, tenantId);
        }

        public async Task<IEnumerable<Role>> GetRolesByUserAsync(int userId, int tenantId)
        {
            return await _roleRepository.GetRolesByUserAsync(userId, tenantId);
        }

        public async Task<int> CreateRoleAsync(Role role)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure creation date is set
                if (role.CreatedDate == default)
                {
                    role.CreatedDate = DateTime.UtcNow;
                }

                int roleId = await _roleRepository.InsertAsync(role);
                await _unitOfWork.CommitAsync();
                return roleId;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure modification date is updated
                role.LastModifiedDate = DateTime.UtcNow;

                bool result = await _roleRepository.UpdateAsync(role);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                bool result = await _roleRepository.DeleteAsync(id, tenantId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<Role>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            return await _roleRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
        }
    }
}
