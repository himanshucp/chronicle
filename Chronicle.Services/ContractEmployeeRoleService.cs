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
    public class ContractEmployeeRoleService : IContractEmployeeRoleService
    {
        private readonly IContractEmployeeRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractEmployeeRoleService(
            IContractEmployeeRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<ContractEmployeeRole>> GetRoleByIdAsync(int roleId, int tenantId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (role == null)
                {
                    return ServiceResult<ContractEmployeeRole>.FailureResult("Role not found");
                }

                return ServiceResult<ContractEmployeeRole>.SuccessResult(role);
            }
            catch (Exception ex)
            {
                return ServiceResult<ContractEmployeeRole>.FailureResult($"Error retrieving role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ContractEmployeeRole>> GetByRoleNameAsync(string roleName, int tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return ServiceResult<ContractEmployeeRole>.FailureResult("Role name cannot be empty");
                }

                var role = await _roleRepository.GetByRoleNameAsync(roleName, tenantId);
                if (role == null)
                {
                    return ServiceResult<ContractEmployeeRole>.FailureResult("Role not found");
                }

                return ServiceResult<ContractEmployeeRole>.SuccessResult(role);
            }
            catch (Exception ex)
            {
                return ServiceResult<ContractEmployeeRole>.FailureResult($"Error retrieving role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployeeRole>>> GetAllRolesAsync(int tenantId)
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync(tenantId);
                return ServiceResult<IEnumerable<ContractEmployeeRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployeeRole>>.FailureResult($"Error retrieving roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployeeRole>>> GetActiveRolesAsync(int tenantId)
        {
            try
            {
                var roles = await _roleRepository.GetActiveRolesAsync(tenantId);
                return ServiceResult<IEnumerable<ContractEmployeeRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployeeRole>>.FailureResult($"Error retrieving active roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<ContractEmployeeRole>>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var roles = await _roleRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<ContractEmployeeRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<ContractEmployeeRole>>.FailureResult($"Error retrieving roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateRoleAsync(ContractEmployeeRole role, int tenantId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(role.RoleName))
                {
                    return ServiceResult<int>.FailureResult("Role name is required");
                }

                // Set tenant ID
                role.TenantID = tenantId;

                // Check if role name already exists within the tenant
                var existingRole = await _roleRepository.GetByRoleNameAsync(role.RoleName, tenantId);
                if (existingRole != null)
                {
                    return ServiceResult<int>.FailureResult("A role with this name already exists");
                }

                // Set default values
                role.CreatedDate = DateTime.UtcNow;
                role.ModifiedDate = DateTime.UtcNow;
                role.IsActive = role.IsActive ?? true;

                _unitOfWork.BeginTransaction();

                int roleId = await _roleRepository.InsertAsync(role);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(roleId, "Role created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateRoleAsync(ContractEmployeeRole role, int tenantId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(role.RoleName))
                {
                    return ServiceResult<bool>.FailureResult("Role name is required");
                }

                // Ensure tenant ID matches
                role.TenantID = tenantId;

                // Check if role exists within the tenant
                var existingRole = await _roleRepository.GetByIdAsync(role.ContractRoleID, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Check if role name is unique within the tenant (excluding current role)
                var roleByName = await _roleRepository.GetByRoleNameAsync(role.RoleName, tenantId);
                if (roleByName != null && roleByName.ContractRoleID != role.ContractRoleID)
                {
                    return ServiceResult<bool>.FailureResult("A role with this name already exists");
                }

                role.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.UpdateAsync(role);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(int roleId, int tenantId)
        {
            try
            {
                // Check if role exists within the tenant
                var existingRole = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // TODO: Check if role is in use by any contracts before deleting
                // This would require checking the Contracts table for any references to this role

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.DeleteAsync(roleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateRoleAsync(int roleId, int tenantId)
        {
            try
            {
                // Check if role exists within the tenant
                var existingRole = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Role not found");
                }

                // Set role as inactive
                existingRole.IsActive = false;
                existingRole.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.UpdateAsync(existingRole);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Role deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating role: {ex.Message}");
            }
        }
    }
}
