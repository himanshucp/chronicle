using Chronicle.Data;
using Chronicle.Data.Extensions;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract = Chronicle.Entities.Contract;

namespace Chronicle.Repositories
{

    public class ContractRepository : DapperRepository<Contract, int>, IContractRepository
    {
        public ContractRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Contracts", "ContractID")
        {
        }

        public async Task<Contract> GetByIdWithEmployeesAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT c.*, 
                       ce.ContractEmployeeID, ce.EmployeeID, ce.RoleID, ce.LineManagerID, 
                       ce.HourlyRate, ce.EstimatedHours, ce.ActualHours, ce.DateActivated, 
                       ce.DateDeactivated, ce.CreatedDate as CECreatedDate, ce.ModifiedDate as CEModifiedDate, 
                       ce.IsActive as CEIsActive, ce.TenantID as CETenantID, ce.ContractID as CEContractID,
                       e.EmployeeID, e.FirstName, e.LastName, e.Email, e.DepartmentID,
                       r.RoleID, r.Name as RoleName,
                       lm.EmployeeID as LMEmployeeID, lm.FirstName as LMFirstName, lm.LastName as LMLastName
                FROM Contracts c
                LEFT JOIN ContractEmployees ce ON c.ContractID = ce.ContractID AND ce.TenantID = @TenantID
                LEFT JOIN Employees e ON ce.EmployeeID = e.EmployeeID AND e.TenantID = @TenantID
                LEFT JOIN Roles r ON ce.RoleID = r.RoleID AND r.TenantID = @TenantID
                LEFT JOIN Employees lm ON ce.LineManagerID = lm.EmployeeID AND lm.TenantID = @TenantID
                WHERE c.ContractID = @ContractID AND c.TenantID = @TenantID";

            var contractDictionary = new Dictionary<int, Contract>();

            var contracts = await _unitOfWork.Connection.QueryAsync<Contract, ContractEmployee, Employee, Role, Employee, Contract>(
                sql,
                (contract, contractEmployee, employee, role, lineManager) =>
                {
                    if (!contractDictionary.TryGetValue(contract.ContractID, out Contract contractEntry))
                    {
                        contractEntry = contract;
                        contractEntry.ContractEmployees = new List<ContractEmployee>();
                        contractDictionary.Add(contract.ContractID, contractEntry);
                    }

                    if (contractEmployee != null && contractEmployee.ContractEmployeeID > 0)
                    {
                        // Check if this contract employee is already added
                        var existingContractEmployee = contractEntry.ContractEmployees
                            .FirstOrDefault(ce => ce.ContractEmployeeID == contractEmployee.ContractEmployeeID);

                        if (existingContractEmployee == null)
                        {
                            contractEmployee.Employee = employee;
                            contractEmployee.Role = role;
                            contractEmployee.LineManager = lineManager;
                            contractEntry.ContractEmployees.Add(contractEmployee);
                        }
                    }

                    return contractEntry;
                },
                new { ContractID = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "ContractEmployeeID,EmployeeID,RoleID,LMEmployeeID");

            return contracts.FirstOrDefault();
        }

        public async Task<Contract> GetByExternalIdAsync(string contractExternalId, int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE ContractExternalID = @ContractExternalID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Contract>(
                sql,
                new { ContractExternalID = contractExternalId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Contract> GetByTitleAsync(string contractTitle, int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE ContractTitle = @ContractTitle AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Contract>(
                sql,
                new { ContractTitle = contractTitle, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Contract>> GetContractsByProjectAsync(int projectId, int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE ProjectID = @ProjectID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Contract>(
                sql,
                new { ProjectID = projectId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Contract>> GetContractsByCompanyAsync(int companyId, int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE CompanyID = @CompanyID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Contract>(
                sql,
                new { CompanyID = companyId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Contract>> GetChildContractsAsync(int parentContractId, int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE ParentContractID = @ParentContractID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QueryAsync<Contract>(
                sql,
                new { ParentContractID = parentContractId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Contract> GetByIdAsync(int id, int tenantId)
        {
            // Use the enhanced method that includes employees
            return await GetByIdWithEmployeesAsync(id, tenantId);
        }

        public override async Task<int> InsertAsync(Contract contract)
        {
            const string sql = @"
                INSERT INTO Contracts (
                    TenantID, ContractExternalID, ContractTitle, ProjectID, SectionID, CompanyID, 
                    CompanyRoleID, ParentContractID, HierarchyLevelID, ContractType, ContractAmount, 
                    RetentionPercentage, RetentionAmount, StartDate, EndDate, SignDate, Status, 
                    ContractManagerID, PaymentTerms, InsuranceRequired, InsuranceVerified, 
                    InsuranceExpiryDate, Location,Notes,CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @ContractExternalID, @ContractTitle, @ProjectID, @SectionID, @CompanyID, 
                    @CompanyRoleID, @ParentContractID, @HierarchyLevelID, @ContractType, @ContractAmount, 
                    @RetentionPercentage, @RetentionAmount, @StartDate, @EndDate, @SignDate, @Status, 
                    @ContractManagerID, @PaymentTerms, @InsuranceRequired, @InsuranceVerified, 
                    @InsuranceExpiryDate,@Location,@Notes, @CreatedDate, @ModifiedDate, 1);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (contract.CreatedDate == null)
            {
                contract.CreatedDate = DateTime.UtcNow;
                contract.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                contract,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Contract contract)
        {
            const string sql = @"
                UPDATE Contracts
                SET ContractExternalID = @ContractExternalID,
                    ContractTitle = @ContractTitle,
                    ProjectID = @ProjectID,
                    SectionID = @SectionID,
                    CompanyID = @CompanyID,
                    CompanyRoleID = @CompanyRoleID,
                    ParentContractID = @ParentContractID,
                    HierarchyLevelID = @HierarchyLevelID,
                    ContractType = @ContractType,
                    ContractAmount = @ContractAmount,
                    RetentionPercentage = @RetentionPercentage,
                    RetentionAmount = @RetentionAmount,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    SignDate = @SignDate,
                    Status = @Status,
                    ContractManagerID = @ContractManagerID,
                    PaymentTerms = @PaymentTerms,
                    InsuranceRequired = @InsuranceRequired,
                    InsuranceVerified = @InsuranceVerified,
                    InsuranceExpiryDate = @InsuranceExpiryDate,
                    Location = @Location,
                    Notes = @Notes,
                    ModifiedDate = @ModifiedDate
                WHERE ContractID = @ContractID AND TenantID = @TenantID";

            // Set modification date
            contract.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                contract,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Contracts WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Contract>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Contract>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    ContractTitle LIKE @SearchTerm OR 
                    ContractExternalID LIKE @SearchTerm OR
                    ContractType LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Contract>(
                "Contracts",
                "ContractTitle",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM Contracts WHERE ContractID = @ContractID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { ContractID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }


}


