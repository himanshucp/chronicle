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
    public class CompanyRoleService : ICompanyRoleService
    {
        private readonly ICompanyRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyRoleService(
            ICompanyRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<CompanyRole>> GetRoleByIdAsync(int roleId, int tenantId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (role == null)
                {
                    return ServiceResult<CompanyRole>.FailureResult("Company role not found");
                }

                return ServiceResult<CompanyRole>.SuccessResult(role);
            }
            catch (Exception ex)
            {
                return ServiceResult<CompanyRole>.FailureResult($"Error retrieving company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CompanyRole>> GetByRoleNameAsync(string roleName, int tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return ServiceResult<CompanyRole>.FailureResult("Role name cannot be empty");
                }

                var role = await _roleRepository.GetByNameAsync(roleName, tenantId);
                if (role == null)
                {
                    return ServiceResult<CompanyRole>.FailureResult("Company role not found");
                }

                return ServiceResult<CompanyRole>.SuccessResult(role);
            }
            catch (Exception ex)
            {
                return ServiceResult<CompanyRole>.FailureResult($"Error retrieving company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<CompanyRole>>> GetAllRolesAsync(int tenantId)
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync(tenantId);
                return ServiceResult<IEnumerable<CompanyRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CompanyRole>>.FailureResult($"Error retrieving company roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<CompanyRole>>> GetActiveRolesAsync(int tenantId)
        {
            try
            {
                var roles = await _roleRepository.GetActiveCompanyRolesAsync(tenantId);
                return ServiceResult<IEnumerable<CompanyRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CompanyRole>>.FailureResult($"Error retrieving active company roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<CompanyRole>>> GetPagedRolesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                {
                    return ServiceResult<PagedResult<CompanyRole>>.FailureResult("Invalid page or page size");
                }

                var roles = await _roleRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<CompanyRole>>.SuccessResult(roles);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<CompanyRole>>.FailureResult($"Error retrieving company roles: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateRoleAsync(CompanyRole role, int tenantId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(role.RoleName))
                {
                    return ServiceResult<int>.FailureResult("Role name is required");
                }

                if (role.RoleName.Length > 100)
                {
                    return ServiceResult<int>.FailureResult("Role name cannot exceed 100 characters");
                }

                // Set tenant ID
                role.TenantID = tenantId;

                // Check if role name is unique
                bool isUnique = await _roleRepository.IsRoleNameUniqueAsync(role.RoleName, tenantId);
                if (!isUnique)
                {
                    return ServiceResult<int>.FailureResult("A company role with this name already exists");
                }

                // Set default values
                role.CreatedDate = DateTime.UtcNow;
                role.ModifiedDate = DateTime.UtcNow;
                role.IsActive = role.IsActive ?? true;

                _unitOfWork.BeginTransaction();

                int roleId = await _roleRepository.InsertAsync(role);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(roleId, "Company role created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateRoleAsync(CompanyRole role, int tenantId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(role.RoleName))
                {
                    return ServiceResult<bool>.FailureResult("Role name is required");
                }

                if (role.RoleName.Length > 100)
                {
                    return ServiceResult<bool>.FailureResult("Role name cannot exceed 100 characters");
                }

                // Ensure tenant ID matches
                role.TenantID = tenantId;

                // Check if role exists
                var existingRole = await _roleRepository.GetByIdAsync(role.CompanyRoleID, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Company role not found");
                }

                // Check if role name is unique (excluding current role)
                bool isUnique = await _roleRepository.IsRoleNameUniqueAsync(role.RoleName, tenantId, role.CompanyRoleID);
                if (!isUnique)
                {
                    return ServiceResult<bool>.FailureResult("A company role with this name already exists");
                }

                role.ModifiedDate = DateTime.UtcNow;
                role.CreatedDate = existingRole.CreatedDate; // Preserve original creation date

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.UpdateAsync(role);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Company role updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(int roleId, int tenantId)
        {
            try
            {
                // Check if role exists
                var existingRole = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Company role not found");
                }

                // TODO: Check if role is in use by any companies before deleting
                // This would require checking the Companies table for any references to this role
                // Example: var companiesUsingRole = await _companyRepository.GetByRoleIdAsync(roleId, tenantId);
                // if (companiesUsingRole.Any()) return error

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.DeleteAsync(roleId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Company role deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateRoleAsync(int roleId, int tenantId)
        {
            try
            {
                // Check if role exists
                var existingRole = await _roleRepository.GetByIdAsync(roleId, tenantId);
                if (existingRole == null)
                {
                    return ServiceResult<bool>.FailureResult("Company role not found");
                }

                if (existingRole.IsActive == false)
                {
                    return ServiceResult<bool>.FailureResult("Company role is already inactive");
                }

                // Set role as inactive
                existingRole.IsActive = false;
                existingRole.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _roleRepository.UpdateAsync(existingRole);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Company role deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating company role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsRoleNameAvailableAsync(string roleName, int tenantId, int? excludeRoleId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return ServiceResult<bool>.FailureResult("Role name cannot be empty");
                }

                bool isAvailable = await _roleRepository.IsRoleNameUniqueAsync(roleName, tenantId, excludeRoleId);

                if (isAvailable)
                {
                    return ServiceResult<bool>.SuccessResult(true, "Role name is available");
                }
                else
                {
                    return ServiceResult<bool>.SuccessResult(false, "Role name is already in use");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error checking role name availability: {ex.Message}");
            }
        }
    }
}
