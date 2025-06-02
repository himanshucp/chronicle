using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(int id, int tenantId);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(int tenantId);
        Task<int> CreateEmployeeAsync(Employee employee, int tenantId);
        Task<bool> UpdateEmployeeAsync(Employee employee, int tenantId);
        Task<bool> DeleteEmployeeAsync(int id, int tenantId);
        Task<Employee> GetEmployeeByEmailAsync(string email, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId, int tenantId);
        Task<PagedResult<Employee>> GetPagedEmployeesAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync(int tenantId);
        Task<PagedResult<Employee>> GetEmployeesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByContractAsync(int contractId, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByEmployeeTypeAsync(string employeeType, int tenantId);
        Task<bool> ActivateEmployeeAsync(int id, int tenantId);
        Task<bool> DeactivateEmployeeAsync(int id, int tenantId);
        Task<bool> UpdateEmployeeSafetyTrainingAsync(int id, bool completed, DateTime? trainingDate, int tenantId);
        Task<bool> UpdateEmployeeLicenseAsync(int id, bool hasLicense, string licenseNumber, DateTime? expiryDate, int tenantId);
    }

}
