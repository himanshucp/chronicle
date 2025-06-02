using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IContractEmployeeService
    {
        Task<ServiceResult<ContractEmployee>> GetContractEmployeeByIdAsync(int contractEmployeeId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByContractIdAsync(int contractId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByEmployeeIdAsync(int employeeId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByRoleIdAsync(int roleId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetByLineManagerIdAsync(int lineManagerId, int tenantId);
        Task<ServiceResult<ContractEmployee>> GetByContractAndEmployeeAsync(int contractId, int employeeId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetActiveByContractIdAsync(int contractId, int tenantId);
        Task<ServiceResult<IEnumerable<ContractEmployee>>> GetActiveByEmployeeIdAsync(int employeeId, int tenantId);
        Task<IEnumerable<ContractEmployee>> GetContractEmployeesAsync(int tenantId);
        Task<ServiceResult<PagedResult<ContractEmployee>>> GetPagedContractEmployeesAsync(int page, int pageSize, int tenantId, string searchTerm = null);
        Task<ServiceResult<int>> CreateContractEmployeeAsync(ContractEmployee contractEmployee, int tenantId);
        Task<ServiceResult<bool>> UpdateAsync(ContractEmployee contractEmployee, int tenantId);
        Task<ServiceResult<bool>> DeleteAsync(int contractEmployeeId, int tenantId);
        Task<ServiceResult<bool>> DeactivateAsync(int contractEmployeeId, int tenantId);
        Task<ServiceResult<bool>> ReactivateAsync(int contractEmployeeId, int tenantId);
    }
}
