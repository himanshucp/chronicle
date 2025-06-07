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
    public class CompanyRoleRepository : DapperRepository<CompanyRole, int>, ICompanyRoleRepository
    {
        public CompanyRoleRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "CompanyRoles", "CompanyRoleID")
        {
        }

        public async Task<CompanyRole> GetByNameAsync(string roleName, int tenantId)
        {
            const string sql = "SELECT * FROM CompanyRoles WHERE RoleName = @RoleName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<CompanyRole>(
                sql,
                new { RoleName = roleName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<CompanyRole>> GetActiveCompanyRolesAsync(int tenantId)
        {
            const string sql = "SELECT * FROM CompanyRoles WHERE TenantID = @TenantID AND IsActive = 1 ORDER BY RoleName";
            return await _unitOfWork.Connection.QueryAsync<CompanyRole>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<bool> IsRoleNameUniqueAsync(string roleName, int tenantId, int? excludeRoleId = null)
        {
            string sql = "SELECT COUNT(*) FROM CompanyRoles WHERE RoleName = @RoleName AND TenantID = @TenantID";
            object parameters;

            if (excludeRoleId.HasValue)
            {
                sql += " AND CompanyRoleID != @ExcludeRoleID";
                parameters = new { RoleName = roleName, TenantID = tenantId, ExcludeRoleID = excludeRoleId.Value };
            }
            else
            {
                parameters = new { RoleName = roleName, TenantID = tenantId };
            }

            int count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                sql,
                parameters,
                _unitOfWork.Transaction);

            return count == 0;
        }

        public override async Task<int> InsertAsync(CompanyRole role)
        {
            const string sql = @"
                INSERT INTO CompanyRoles (
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

        public override async Task<bool> UpdateAsync(CompanyRole role)
        {
            const string sql = @"
                UPDATE CompanyRoles
                SET RoleName = @RoleName,
                    Description = @Description,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE CompanyRoleID = @CompanyRoleID AND TenantID = @TenantID";

            // Set modification date
            role.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                role,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<CompanyRole> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM CompanyRoles WHERE CompanyRoleID = @CompanyRoleID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<CompanyRole>(
                sql,
                new { CompanyRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<CompanyRole>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM CompanyRoles WHERE TenantID = @TenantID ORDER BY RoleName";
            return await _unitOfWork.Connection.QueryAsync<CompanyRole>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<CompanyRole>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
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

            return await _unitOfWork.Connection.QueryPagedAsync<CompanyRole>(
                "CompanyRoles",
                "RoleName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM CompanyRoles WHERE CompanyRoleID = @CompanyRoleID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { CompanyRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
