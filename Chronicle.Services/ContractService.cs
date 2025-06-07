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
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IContractEmployeeRepository _contractEmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(
            IContractRepository contractRepository,
            IContractEmployeeRepository contractEmployeeRepository,
            IUnitOfWork unitOfWork)
        {
            _contractRepository = contractRepository;
            _contractEmployeeRepository = contractEmployeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<Contract>> GetContractByIdAsync(int contractId, int tenantId)
        {
            try
            {
                // Use the enhanced repository method that fetches contract with all related data
                var contract = await _contractRepository.GetByIdWithEmployeesAsync(contractId, tenantId);
                if (contract == null)
                {
                    return ServiceResult<Contract>.FailureResult("Contract not found");
                }

                return ServiceResult<Contract>.SuccessResult(contract);
            }
            catch (Exception ex)
            {
                return ServiceResult<Contract>.FailureResult($"Error retrieving contract: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Contract>> GetByExternalIdAsync(string contractExternalId, int tenantId)
        {
            try
            {
                var contract = await _contractRepository.GetByExternalIdAsync(contractExternalId, tenantId);
                if (contract == null)
                {
                    return ServiceResult<Contract>.FailureResult("Contract not found");
                }

                return ServiceResult<Contract>.SuccessResult(contract);
            }
            catch (Exception ex)
            {
                return ServiceResult<Contract>.FailureResult($"Error retrieving contract: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Contract>> GetByTitleAsync(string contractTitle, int tenantId)
        {
            try
            {
                var contract = await _contractRepository.GetByTitleAsync(contractTitle, tenantId);
                if (contract == null)
                {
                    return ServiceResult<Contract>.FailureResult("Contract not found");
                }

                return ServiceResult<Contract>.SuccessResult(contract);
            }
            catch (Exception ex)
            {
                return ServiceResult<Contract>.FailureResult($"Error retrieving contract: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Contract>>> GetContractsByProjectAsync(int projectId, int tenantId)
        {
            try
            {
                var contracts = await _contractRepository.GetContractsByProjectAsync(projectId, tenantId);
                return ServiceResult<IEnumerable<Contract>>.SuccessResult(contracts);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Contract>>.FailureResult($"Error retrieving contracts: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Contract>>> GetContractsByCompanyAsync(int companyId, int tenantId)
        {
            try
            {
                var contracts = await _contractRepository.GetContractsByCompanyAsync(companyId, tenantId);
                return ServiceResult<IEnumerable<Contract>>.SuccessResult(contracts);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Contract>>.FailureResult($"Error retrieving contracts: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Contract>>> GetChildContractsAsync(int parentContractId, int tenantId)
        {
            try
            {
                var contracts = await _contractRepository.GetChildContractsAsync(parentContractId, tenantId);
                return ServiceResult<IEnumerable<Contract>>.SuccessResult(contracts);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Contract>>.FailureResult($"Error retrieving child contracts: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateContractAsync(Contract contract, int tenantId)
        {
            try
            {
                // Set tenant ID
                contract.TenantID = tenantId;

                // Check if contract external ID already exists within the tenant
                if (!string.IsNullOrEmpty(contract.ContractExternalID))
                {
                    var existingContractByExternalId = await _contractRepository.GetByExternalIdAsync(contract.ContractExternalID, tenantId);
                    if (existingContractByExternalId != null)
                    {
                        return ServiceResult<int>.FailureResult("Contract with this external ID already exists");
                    }
                }

                // Check if contract title already exists within the tenant
                var existingContractByTitle = await _contractRepository.GetByTitleAsync(contract.ContractTitle, tenantId);
                if (existingContractByTitle != null)
                {
                    return ServiceResult<int>.FailureResult("Contract with this title already exists");
                }

                // Set default values
                contract.CreatedDate = DateTime.UtcNow;
                contract.ModifiedDate = DateTime.UtcNow;
                contract.IsActive = true;

                _unitOfWork.BeginTransaction();

                int contractId = await _contractRepository.InsertAsync(contract);

                // Insert contract employees if any
                if (contract.ContractEmployees != null && contract.ContractEmployees.Any())
                {
                    foreach (ContractEmployee contractEmployee in contract.ContractEmployees)
                    {
                        contractEmployee.ContractID = contractId;
                        contractEmployee.TenantID = contract.TenantID;
                        contractEmployee.CreatedDate = DateTime.UtcNow;
                        contractEmployee.ModifiedDate = DateTime.UtcNow;
                        contractEmployee.IsActive = true;

                        int contractEmployeeID = await _contractEmployeeRepository.InsertAsync(contractEmployee);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(contractId, "Contract created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating contract: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Contract contract, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                contract.TenantID = tenantId;

                // Check if contract exists within the tenant
                var existingContract = await _contractRepository.GetByIdAsync(contract.ContractID, tenantId);
                if (existingContract == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract not found");
                }

                // Check if external ID is unique within the tenant
                if (!string.IsNullOrEmpty(contract.ContractExternalID))
                {
                    var contractByExternalId = await _contractRepository.GetByExternalIdAsync(contract.ContractExternalID, tenantId);
                    if (contractByExternalId != null && contractByExternalId.ContractID != contract.ContractID)
                    {
                        return ServiceResult<bool>.FailureResult("Contract external ID already exists");
                    }
                }

                // Check if title is unique within the tenant
                var contractByTitle = await _contractRepository.GetByTitleAsync(contract.ContractTitle, tenantId);
                if (contractByTitle != null && contractByTitle.ContractID != contract.ContractID)
                {
                    return ServiceResult<bool>.FailureResult("Contract title already exists");
                }

                contract.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _contractRepository.UpdateAsync(contract);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating contract: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateWithEmployeesAsync(Contract contract, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                contract.TenantID = tenantId;

                // Check if contract exists within the tenant
                var existingContract = await _contractRepository.GetByIdAsync(contract.ContractID, tenantId);
                if (existingContract == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract not found");
                }

                // Check if external ID is unique within the tenant
                if (!string.IsNullOrEmpty(contract.ContractExternalID))
                {
                    var contractByExternalId = await _contractRepository.GetByExternalIdAsync(contract.ContractExternalID, tenantId);
                    if (contractByExternalId != null && contractByExternalId.ContractID != contract.ContractID)
                    {
                        return ServiceResult<bool>.FailureResult("Contract external ID already exists");
                    }
                }

                // Check if title is unique within the tenant
                var contractByTitle = await _contractRepository.GetByTitleAsync(contract.ContractTitle, tenantId);
                if (contractByTitle != null && contractByTitle.ContractID != contract.ContractID)
                {
                    return ServiceResult<bool>.FailureResult("Contract title already exists");
                }

                contract.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                // Update the contract
                bool contractUpdateResult = await _contractRepository.UpdateAsync(contract);
                if (!contractUpdateResult)
                {
                    throw new Exception("Failed to update contract");
                }

                // Handle contract employees update
                await UpdateContractEmployeesInTransactionAsync(contract.ContractID, contract.ContractEmployees, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Contract and employees updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating contract with employees: {ex.Message}");
            }
        }

        private async Task UpdateContractEmployeesInTransactionAsync(int contractId, ICollection<ContractEmployee> newEmployees, int tenantId)
        {
            try
            {
                // Get existing employees for this contract
                var existingEmployees = await _contractEmployeeRepository.GetByContractIdAsync(contractId, tenantId);
                var existingEmployeesList = existingEmployees.ToList();

                if (newEmployees == null)
                {
                    newEmployees = new List<ContractEmployee>();
                }

                // Find employees to delete (exist in DB but not in new list)
                var employeesToDelete = existingEmployeesList.Where(existing =>
                    !newEmployees.Any(newEmp => newEmp.EmployeeID == existing.EmployeeID)).ToList();

                // Find employees to add (in new list but don't exist in DB)
                var employeesToAdd = newEmployees.Where(newEmp =>
                    !existingEmployeesList.Any(existing => existing.EmployeeID == newEmp.EmployeeID)).ToList();

                // Find employees to update (exist in both lists)
                var employeesToUpdate = newEmployees.Where(newEmp =>
                    existingEmployeesList.Any(existing => existing.EmployeeID == newEmp.EmployeeID)).ToList();

                // Delete removed employees
                foreach (var employeeToDelete in employeesToDelete)
                {
                    await _contractEmployeeRepository.DeleteAsync(employeeToDelete.ContractEmployeeID, tenantId);
                }

                // Update existing employees
                foreach (var newEmployee in employeesToUpdate)
                {
                    var existingEmployee = existingEmployeesList.First(e => e.EmployeeID == newEmployee.EmployeeID);

                    // Update the existing employee with new data
                    existingEmployee.RoleID = newEmployee.RoleID;
                    existingEmployee.LineManagerID = newEmployee.LineManagerID;
                    existingEmployee.DateActivated = newEmployee.DateActivated;
                    existingEmployee.DateDeactivated = newEmployee.DateDeactivated;
                    existingEmployee.IsActive = newEmployee.IsActive;
                    existingEmployee.ModifiedDate = DateTime.UtcNow;

                    await _contractEmployeeRepository.UpdateAsync(existingEmployee);
                }

                // Add new employees
                foreach (var newEmployee in employeesToAdd)
                {
                    newEmployee.ContractID = contractId;
                    newEmployee.TenantID = tenantId;
                    newEmployee.CreatedDate = DateTime.UtcNow;
                    newEmployee.ModifiedDate = DateTime.UtcNow;
                    newEmployee.IsActive = true;

                    await _contractEmployeeRepository.InsertAsync(newEmployee);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating contract employees: {ex.Message}", ex);
            }
        }




        public async Task<ServiceResult<bool>> DeleteAsync(int contractId, int tenantId)
        {
            try
            {
                // Check if contract exists within the tenant
                var existingContract = await _contractRepository.GetByIdAsync(contractId, tenantId);
                if (existingContract == null)
                {
                    return ServiceResult<bool>.FailureResult("Contract not found");
                }

                _unitOfWork.BeginTransaction();

                // Delete associated contract employees first
                var contractEmployees = await _contractEmployeeRepository.GetByContractIdAsync(contractId, tenantId);
                foreach (var contractEmployee in contractEmployees)
                {
                    await _contractEmployeeRepository.DeleteAsync(contractEmployee.ContractEmployeeID, tenantId);
                }

                // Delete the contract
                bool result = await _contractRepository.DeleteAsync(contractId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Contract deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting contract: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Contract>> GetContractsAsync(int tenantId)
        {
            return await _contractRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<Contract>>> GetPagedContractsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var contracts = await _contractRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<Contract>>.SuccessResult(contracts);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<Contract>>.FailureResult($"Error retrieving contracts: {ex.Message}");
            }
        }
    }


}
