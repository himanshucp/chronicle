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

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id, int tenantId)
        {
            return await _employeeRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(int tenantId)
        {
            return await _employeeRepository.GetAllAsync(tenantId);
        }

        public async Task<int> CreateEmployeeAsync(Employee employee, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure employee belongs to the right tenant
                employee.TenantID = tenantId;

                // Validate employee data
                if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName))
                {
                    throw new ArgumentException("Employee first and last name are required");
                }

                // Validate that the company exists and belongs to this tenant
                if (employee.CompanyID > 0)
                {
                    var company = await _companyRepository.GetByIdAsync(employee.CompanyID, tenantId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"Company with ID {employee.CompanyID} not found or does not belong to this tenant");
                    }
                }

                // Check if employee with same email already exists in this tenant
                if (!string.IsNullOrEmpty(employee.Email))
                {
                    var existingEmail = await _employeeRepository.GetByEmailAsync(employee.Email, tenantId);
                    if (existingEmail != null)
                    {
                        throw new InvalidOperationException($"An employee with email '{employee.Email}' already exists");
                    }
                }

                // Set audit fields
                employee.CreatedDate = DateTime.UtcNow;
                employee.ModifiedDate = DateTime.UtcNow;
                employee.IsActive = true;

                int id = await _employeeRepository.InsertAsync(employee);
                await _unitOfWork.CommitAsync();
                return id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate employee data
                if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName))
                {
                    throw new ArgumentException("Employee first and last name are required");
                }

                // Get existing employee and verify it belongs to this tenant
                var existingEmployee = await _employeeRepository.GetByIdAsync(employee.EmployeeID, tenantId);
                if (existingEmployee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {employee.EmployeeID} not found");
                }

                // Validate that the company exists and belongs to this tenant
                if (employee.CompanyID > 0)
                {
                    var company = await _companyRepository.GetByIdAsync(employee.CompanyID, tenantId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"Company with ID {employee.CompanyID} not found or does not belong to this tenant");
                    }
                }

                // Ensure tenant ID cannot be changed
                employee.TenantID = tenantId;

                // Check if another employee with same email exists in this tenant
                if (!string.IsNullOrEmpty(employee.Email))
                {
                    var emailCheck = await _employeeRepository.GetByEmailAsync(employee.Email, tenantId);
                    if (emailCheck != null && emailCheck.EmployeeID != employee.EmployeeID)
                    {
                        throw new InvalidOperationException($"Another employee with email '{employee.Email}' already exists");
                    }
                }

                // Set audit fields
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify employee belongs to this tenant
                var employee = await _employeeRepository.GetByIdAsync(id, tenantId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {id} not found");
                }

                // Perform soft delete by setting IsActive to false
                employee.IsActive = false;
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Employee> GetEmployeeByEmailAsync(string email, int tenantId)
        {
            return await _employeeRepository.GetByEmailAsync(email, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId, int tenantId)
        {
            // Verify company belongs to this tenant
            var company = await _companyRepository.GetByIdAsync(companyId, tenantId);
            if (company == null)
            {
                throw new InvalidOperationException($"Company with ID {companyId} not found or does not belong to this tenant");
            }

            return await _employeeRepository.GetByCompanyAsync(companyId, tenantId);
        }

        public async Task<PagedResult<Employee>> GetPagedEmployeesAsync(int page, int pageSize, string searchTerm, int tenantId)
        {
            return await _employeeRepository.GetPagedAsync(page, pageSize, searchTerm, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync(int tenantId)
        {
            return await _employeeRepository.GetActiveEmployeesAsync(tenantId);
        }

        public async Task<PagedResult<Employee>> GetEmployeesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            return await _employeeRepository.GetEmployeesWithExpiringLicensesAsync(daysThreshold, page, pageSize, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByContractAsync(int contractId, int tenantId)
        {
            return await _employeeRepository.GetEmployeesByContractAsync(contractId, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position, int tenantId)
        {
            return await _employeeRepository.GetEmployeesByPositionAsync(position, tenantId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByEmployeeTypeAsync(string employeeType, int tenantId)
        {
            return await _employeeRepository.GetEmployeesByEmployeeTypeAsync(employeeType, tenantId);
        }

        public async Task<bool> ActivateEmployeeAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify employee belongs to this tenant
                var employee = await _employeeRepository.GetByIdAsync(id, tenantId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {id} not found");
                }

                employee.IsActive = true;
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeactivateEmployeeAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify employee belongs to this tenant
                var employee = await _employeeRepository.GetByIdAsync(id, tenantId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {id} not found");
                }

                employee.IsActive = false;
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateEmployeeSafetyTrainingAsync(int id, bool completed, DateTime? trainingDate, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify employee belongs to this tenant
                var employee = await _employeeRepository.GetByIdAsync(id, tenantId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {id} not found");
                }

                employee.SafetyTrainingCompleted = completed;
                employee.SafetyTrainingDate = trainingDate;
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateEmployeeLicenseAsync(int id, bool hasLicense, string licenseNumber, DateTime? expiryDate, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify employee belongs to this tenant
                var employee = await _employeeRepository.GetByIdAsync(id, tenantId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"Employee with ID {id} not found");
                }

                employee.HasConstructionLicense = hasLicense;
                employee.LicenseNumber = licenseNumber;
                employee.LicenseExpiryDate = expiryDate;
                employee.ModifiedDate = DateTime.UtcNow;

                bool result = await _employeeRepository.UpdateAsync(employee);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }

}
