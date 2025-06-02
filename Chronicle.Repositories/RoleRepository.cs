using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Data.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// Role repository implementation
    /// </summary>
    public class RoleRepository : DapperRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Roles", "RoleId")
        {
        }

        public override async Task<int> InsertAsync(Role role)
        {
            const string sql = @"
            INSERT INTO Roles (Name, Description, CreatedDate, LastModifiedDate, TenantID)
            VALUES (@Name, @Description, @CreatedDate, @LastModifiedDate, @TenantID);
            SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (role.CreatedDate == default)
            {
                role.CreatedDate = DateTime.UtcNow;
            }

            // Set modification date to match creation date for new records
            role.LastModifiedDate = role.CreatedDate;

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                role,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Role role)
        {
            const string sql = @"
            UPDATE Roles 
            SET Name = @Name,
                Description = @Description,
                LastModifiedDate = @LastModifiedDate
            WHERE RoleId = @RoleId
            AND TenantID = @TenantID";

            // Set modification date
            role.LastModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                role,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<Role> GetByNameAsync(string name, int tenantId)
        {
            const string sql = "SELECT * FROM Roles WHERE Name = @Name AND TenantID = @TenantId";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Role>(
                sql,
                new { Name = name, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Role>> GetRolesByUserAsync(int userId, int tenantId)
        {
            const string sql = @"
            SELECT r.*
            FROM Roles r
            JOIN UserRoles ur ON r.RoleId = ur.RoleId
            WHERE ur.UserId = @UserId
            AND r.TenantID = @TenantId";

            return await _unitOfWork.Connection.QueryAsync<Role>(
                sql,
                new { UserId = userId, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<PagedResult<Role>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantId";
            object parameters = new { TenantId = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += " AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)";
                parameters = new
                {
                    TenantId = tenantId,
                    SearchTerm = $"%{searchTerm}%"
                };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Role>(
                "Roles",
                "Name",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }
    }
}
