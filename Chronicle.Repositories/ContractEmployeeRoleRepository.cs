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
    public class ContractEmployeeRoleRepository : DapperRepository<ContractEmployeeRole, int>, IContractEmployeeRoleRepository
    {
        public ContractEmployeeRoleRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "ContractEmployeeRoles", "ContractRoleID")
        {
        }

        public async Task<ContractEmployeeRole> GetByRoleNameAsync(string roleName, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployeeRoles WHERE RoleName = @RoleName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContractEmployeeRole>(
                sql,
                new { RoleName = roleName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<ContractEmployeeRole> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployeeRoles WHERE ContractRoleID = @ContractRoleID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContractEmployeeRole>(
                sql,
                new { ContractRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployeeRole>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployeeRoles WHERE TenantID = @TenantID ORDER BY RoleName";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployeeRole>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<ContractEmployeeRole>> GetActiveRolesAsync(int tenantId)
        {
            const string sql = "SELECT * FROM ContractEmployeeRoles WHERE TenantID = @TenantID AND IsActive = 1 ORDER BY RoleName";
            return await _unitOfWork.Connection.QueryAsync<ContractEmployeeRole>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(ContractEmployeeRole role)
        {
            const string sql = @"
                INSERT INTO ContractEmployeeRoles (
                    TenantID, RoleName, Description, CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @RoleName, @Description, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (role.CreatedDate == null)
            {
                role.CreatedDate = DateTime.UtcNow;
                role.ModifiedDate = DateTime.UtcNow;
            }

            // Set IsActive to true if not set
            if (role.IsActive == null)
            {
                role.IsActive = true;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                role,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(ContractEmployeeRole role)
        {
            const string sql = @"
                UPDATE ContractEmployeeRoles
                SET RoleName = @RoleName,
                    Description = @Description,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE ContractRoleID = @ContractRoleID AND TenantID = @TenantID";

            // Set modification date
            role.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                role,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<PagedResult<ContractEmployeeRole>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    RoleName LIKE @SearchTerm OR 
                    Description LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<ContractEmployeeRole>(
                "ContractEmployeeRoles",
                "RoleName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM ContractEmployeeRoles WHERE ContractRoleID = @ContractRoleID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { ContractRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
