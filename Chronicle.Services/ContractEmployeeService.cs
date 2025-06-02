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
    public class ContractEmployeeService : IContractEmployeeService
    {
        private readonly IContractEmployeeRepository _contractEmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractEmployeeService(
            IContractEmployeeRepository contractEmployeeRepository,
            IUnitOfWork unitOfWork)
        {
            _contractEmployeeRepository = contractEmployeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<ContractEmployee>> GetContractEmployeeByIdAsync(int contractEmployeeId, int tenantId)
        {
            try
            {
                var contractEmployee = await _contractEmployeeRepository.GetByIdAsync(contractEmployeeId, tenantId);
                if (contractEmployee == null)
                {
                    return ServiceResult<ContractEmployee>.FailureResult("Contract employee not found");
                }

                return ServiceResult<ContractEmployee>.SuccessResult(contractEmployee);
            }
            catch (Exception ex)
            {
                return ServiceResult<ContractEmployee>.FailureResult($"Error retrieving contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByContractIdAsync(int contractId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetByContractIdAsync(contractId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByEmployeeIdAsync(int employeeId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetByEmployeeIdAsync(employeeId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByRoleIdAsync(int roleId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetByRoleIdAsync(roleId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByLineManagerIdAsync(int lineManagerId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetByLineManagerIdAsync(lineManagerId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ContractEmployee>> GetByContractAndEmployeeAsync(int contractId, int employeeId, int tenantId)
        {
            try
            {
                var contractEmployee = await _contractEmployeeRepository.GetByContractAndEmployeeAsync(contractId, employeeId, tenantId);
                if (contractEmployee == null)
                {
                    return ServiceResult<ContractEmployee>.FailureResult("Contract employee not found");
                }

                return ServiceResult<ContractEmployee>.SuccessResult(contractEmployee);
            }
            catch (Exception ex)
            {
                return ServiceResult<ContractEmployee>.FailureResult($"Error retrieving contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetActiveByContractIdAsync(int contractId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetActiveByContractIdAsync(contractId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving active contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ContractEmployee>>> GetActiveByEmployeeIdAsync(int employeeId, int tenantId)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetActiveByEmployeeIdAsync(employeeId, tenantId);
                return ServiceResult<IEnumerable<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ContractEmployee>>.FailureResult($"Error retrieving active contract employees: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ContractEmployee>> GetContractEmployeesAsync(int tenantId)
        {
            return await _contractEmployeeRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<ContractEmployee>>> GetPagedContractEmployeesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var contractEmployees = await _contractEmployeeRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<ContractEmployee>>.SuccessResult(contractEmployees);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<ContractEmployee>>.FailureResult($"Error retrieving contract employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateContractEmployeeAsync(ContractEmployee contractEmployee, int tenantId)
        {
            try
            {
                // Set tenant ID
                //contractEmployee.TenantID = tenantId;

                // Check if contract employee already exists for this contract and employee combination
                var existingContractEmployee = await _contractEmployeeRepository.GetByContractAndEmployeeAsync(
                    contractEmployee.ContractID, contractEmployee.EmployeeID, tenantId);
                if (existingContractEmployee != null)
                {
                    return ServiceResult<int>.FailureResult("Employee is already assigned to this contract");
                }

                // Set default values
                contractEmployee.CreatedDate = DateTime.UtcNow;
                contractEmployee.ModifiedDate = DateTime.UtcNow;
                contractEmployee.IsActive = true;

                if (contractEmployee.DateActivated == DateTime.MinValue)
                {
                    contractEmployee.DateActivated = DateTime.UtcNow;
                }

                _unitOfWork.BeginTransaction();

                int contractEmployeeId = await _contractEmployeeRepository.InsertAsync(contractEmployee);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(contractEmployeeId, "Contract employee created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(ContractEmployee contractEmployee, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                //contractEmployee.TenantID = tenantId;

                // Check if contract employee exists within the tenant
                var existingContractEmployee = await _contractEmployeeRepository.GetByIdAsync(contractEmployee.ContractEmployeeID, tenantId);
                if (existingContractEmployee == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract employee not found");
                }

                // Check if another contract employee exists for this contract and employee combination
                var contractEmployeeByIds = await _contractEmployeeRepository.GetByContractAndEmployeeAsync(
                    contractEmployee.ContractID, contractEmployee.EmployeeID, tenantId);
                if (contractEmployeeByIds != null && contractEmployeeByIds.ContractEmployeeID != contractEmployee.ContractEmployeeID)
                {
                    return ServiceResult<bool>.FailureResult("Employee is already assigned to this contract");
                }

                contractEmployee.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _contractEmployeeRepository.UpdateAsync(contractEmployee);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract employee updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int contractEmployeeId, int tenantId)
        {
            try
            {
                // Check if contract employee exists within the tenant
                var existingContractEmployee = await _contractEmployeeRepository.GetByIdAsync(contractEmployeeId, tenantId);
                if (existingContractEmployee == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract employee not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _contractEmployeeRepository.DeleteAsync(contractEmployeeId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract employee deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateAsync(int contractEmployeeId, int tenantId)
        {
            try
            {
                var contractEmployee = await _contractEmployeeRepository.GetByIdAsync(contractEmployeeId, tenantId);
                if (contractEmployee == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract employee not found");
                }

                contractEmployee.IsActive = false;
                contractEmployee.DateDeactivated = DateTime.UtcNow;
                contractEmployee.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _contractEmployeeRepository.UpdateAsync(contractEmployee);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract employee deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating contract employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReactivateAsync(int contractEmployeeId, int tenantId)
        {
            try
            {
                var contractEmployee = await _contractEmployeeRepository.GetByIdAsync(contractEmployeeId, tenantId);
                if (contractEmployee == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract employee not found");
                }

                contractEmployee.IsActive = true;
                contractEmployee.DateDeactivated = null;
                contractEmployee.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _contractEmployeeRepository.UpdateAsync(contractEmployee);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract employee reactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reactivating contract employee: {ex.Message}");
            }
        }
    }
}
