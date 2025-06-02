using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee, int>
    {
        Task<Employee> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<Employee>> GetAllAsync(int tenantId);
        Task<Employee> GetByEmailAsync(string email, int tenantId);
        Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId, int tenantId);
        Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync(int tenantId);
        Task<PagedResult<Employee>> GetEmployeesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByContractAsync(int contractId, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position, int tenantId);
        Task<IEnumerable<Employee>> GetEmployeesByEmployeeTypeAsync(string employeeType, int tenantId);
    }
}
