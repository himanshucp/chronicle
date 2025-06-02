using Chronicle.Data.Extensions;
using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public class ContractEmployeeRepository : DapperRepository<ContractEmployee, int>, IContractEmployeeRepository
    {
        public ContractEmployeeRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "ContractEmployees", "ContractEmployeeID")
        {
        }

        public async Task<ContractEmployee> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE ContractEmployeeID = @ContractEmployeeID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContractEmployee>(
                sql,
                new { ContractEmployeeID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetByContractIdAsync(int contractId, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE ContractID = @ContractID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { ContractID = contractId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetByEmployeeIdAsync(int employeeId, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE EmployeeID = @EmployeeID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { EmployeeID = employeeId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetByRoleIdAsync(int roleId, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE RoleID = @RoleID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetByLineManagerIdAsync(int lineManagerId, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE LineManagerID = @LineManagerID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { LineManagerID = lineManagerId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<ContractEmployee> GetByContractAndEmployeeAsync(int contractId, int employeeId, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE ContractID = @ContractID AND EmployeeID = @EmployeeID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContractEmployee>(
                sql,
                new { ContractID = contractId, EmployeeID = employeeId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetActiveByContractIdAsync(int contractId, int tenantId)
        {
            const string sql = @"SELECT * FROM ContractEmployees 
                                WHERE ContractID = @ContractID 
                                AND TenantID = @TenantID 
                                AND IsActive = 1 
                                AND (DateDeactivated IS NULL OR DateDeactivated > GETUTCDATE())";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { ContractID = contractId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetActiveByEmployeeIdAsync(int employeeId, int tenantId)
        {
            const string sql = @"SELECT * FROM ContractEmployees 
                                WHERE EmployeeID = @EmployeeID 
                                AND TenantID = @TenantID 
                                AND IsActive = 1 
                                AND (DateDeactivated IS NULL OR DateDeactivated > GETUTCDATE())";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { EmployeeID = employeeId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployee>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployees WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployee>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<ContractEmployee>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    CAST(ContractID AS NVARCHAR) LIKE @SearchTerm OR 
                    CAST(EmployeeID AS NVARCHAR) LIKE @SearchTerm OR
                    CAST(RoleID AS NVARCHAR) LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<ContractEmployee>(
                "ContractEmployees",
                "ContractEmployeeID",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(ContractEmployee contractEmployee)
        {
            const string sql = @"
                INSERT INTO ContractEmployees (
                    TenantID, ContractID, EmployeeID, RoleID, LineManagerID, HourlyRate, 
                    EstimatedHours, ActualHours, DateActivated, DateDeactivated, 
                    CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @ContractID, @EmployeeID, @RoleID, @LineManagerID, @HourlyRate, 
                    @EstimatedHours, @ActualHours, @DateActivated, @DateDeactivated, 
                    @CreatedDate, @ModifiedDate, 1);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (contractEmployee.CreatedDate == null)
            {
                contractEmployee.CreatedDate = DateTime.UtcNow;
                contractEmployee.ModifiedDate = DateTime.UtcNow;
            }

            // Set activation date if not set
            if (contractEmployee.DateActivated == DateTime.MinValue)
            {
                contractEmployee.DateActivated = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                contractEmployee,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(ContractEmployee contractEmployee)
        {
            const string sql = @"
                UPDATE ContractEmployees
                SET ContractID = @ContractID,
                    EmployeeID = @EmployeeID,
                    RoleID = @RoleID,
                    LineManagerID = @LineManagerID,
                    HourlyRate = @HourlyRate,
                    EstimatedHours = @EstimatedHours,
                    ActualHours = @ActualHours,
                    DateActivated = @DateActivated,
                    DateDeactivated = @DateDeactivated,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE ContractEmployeeID = @ContractEmployeeID AND TenantID = @TenantID";

            // Set modification date
            contractEmployee.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                contractEmployee,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM ContractEmployees WHERE ContractEmployeeID = @ContractEmployeeID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { ContractEmployeeID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
