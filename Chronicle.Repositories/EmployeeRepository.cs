using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Chronicle.Repositories
{
    public class EmployeeRepository : DapperRepository<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Employees", "EmployeeID")
        {
        }

        public async Task<Employee> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Employee>(
                sql,
                new { EmployeeID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Employee> GetByEmailAsync(string email, int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE Email = @Email AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Employee>(
                sql,
                new { Email = email, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetByCompanyAsync(int companyId, int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE CompanyID = @CompanyID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { CompanyID = companyId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string searchTerm = null, int tenantId = 0)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    TenantID = @TenantID AND (
                    FirstName LIKE @SearchTerm OR 
                    LastName LIKE @SearchTerm OR
                    Email LIKE @SearchTerm OR
                    Phone LIKE @SearchTerm OR
                    Position LIKE @SearchTerm OR
                    EmployeeType LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Employee>(
                "Employees",
                "LastName, FirstName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Employee>> GetEmployeesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            var currentDate = DateTime.UtcNow;
            var expiryDate = currentDate.AddDays(daysThreshold);

            string whereClause = @"
                LicenseExpiryDate IS NOT NULL AND 
                LicenseExpiryDate <= @ExpiryDate AND 
                IsActive = 1 AND 
                TenantID = @TenantID";

            var parameters = new { ExpiryDate = expiryDate, TenantID = tenantId };

            return await _unitOfWork.Connection.QueryPagedAsync<Employee>(
                "Employees",
                "LastName, FirstName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByContractAsync(int contractId, int tenantId)
        {
            const string sql = @"
                SELECT e.* FROM Employees e
                INNER JOIN ContractEmployees ce ON e.EmployeeID = ce.EmployeeID
                INNER JOIN Contracts c ON ce.ContractID = c.ContractID
                WHERE ce.ContractID = @ContractID AND e.TenantID = @TenantID AND c.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { ContractID = contractId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(string position, int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE Position = @Position AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { Position = position, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByEmployeeTypeAsync(string employeeType, int tenantId)
        {
            const string sql = "SELECT * FROM Employees WHERE EmployeeType = @EmployeeType AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Employee>(
                sql,
                new { EmployeeType = employeeType, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Employee employee)
        {
            const string sql = @"
                INSERT INTO Employees (
                    TenantID, CompanyID, DepartmentID, FirstName, LastName, Email, 
                    Phone, DateOfBirth, HireDate, Position, EmployeeType, 
                    EmergencyContactName, EmergencyContactPhone, HasConstructionLicense, 
                    LicenseNumber, LicenseExpiryDate, SafetyTrainingCompleted, 
                    SafetyTrainingDate, CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @CompanyID, @DepartmentID, @FirstName, @LastName, @Email, 
                    @Phone, @DateOfBirth, @HireDate, @Position, @EmployeeType, 
                    @EmergencyContactName, @EmergencyContactPhone, @HasConstructionLicense, 
                    @LicenseNumber, @LicenseExpiryDate, @SafetyTrainingCompleted, 
                    @SafetyTrainingDate, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (employee.CreatedDate == null)
            {
                employee.CreatedDate = DateTime.UtcNow;
                employee.ModifiedDate = DateTime.UtcNow;
            }

            // Set IsActive to true by default if not specified
            if (employee.IsActive == null)
            {
                employee.IsActive = true;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                employee,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Employee employee)
        {
            const string sql = @"
                UPDATE Employees
                SET TenantID = @TenantID,
                    CompanyID = @CompanyID,
                    DepartmentID = @DepartmentID,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Phone = @Phone,
                    DateOfBirth = @DateOfBirth,
                    HireDate = @HireDate,
                    Position = @Position,
                    EmployeeType = @EmployeeType,
                    EmergencyContactName = @EmergencyContactName,
                    EmergencyContactPhone = @EmergencyContactPhone,
                    HasConstructionLicense = @HasConstructionLicense,
                    LicenseNumber = @LicenseNumber,
                    LicenseExpiryDate = @LicenseExpiryDate,
                    SafetyTrainingCompleted = @SafetyTrainingCompleted,
                    SafetyTrainingDate = @SafetyTrainingDate,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE EmployeeID = @EmployeeID";

            // Set modification date
            employee.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                employee,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Implementing soft delete
            const string sql = @"
                UPDATE Employees 
                SET IsActive = 0, 
                    ModifiedDate = @ModifiedDate 
                WHERE EmployeeID = @EmployeeID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { EmployeeID = id, ModifiedDate = DateTime.UtcNow },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }

}
