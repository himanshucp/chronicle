using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.UsersRepositories
{
    public class UserRoleRepository : DapperRepository<UserRole, int>, IUserRoleRepository
    {
        public UserRoleRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "UserRoles", "UserRoleID")
        {
        }

        public async Task<UserRole> GetByIdWithDetailsAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName, u.IsActive as UserIsActive,
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription, r.IsActive as RoleIsActive
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE ur.UserRoleID = @UserRoleID AND ur.TenantID = @TenantID";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, Role, UserRole>(
                sql,
                (userRole, user, role) =>
                {
                    userRole.User = user;
                    userRole.Role = role;
                    return userRole;
                },
                new { UserRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId,RoleID");

            return userRoles.FirstOrDefault();
        }

        public async Task<UserRole> GetByUserAndRoleAsync(int userId, int roleId, int tenantId)
        {
            const string sql = @"
                SELECT * FROM UserRoles 
                WHERE UserID = @UserID AND RoleID = @RoleID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserRole>(
                sql,
                new { UserID = userId, RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription
                FROM UserRoles ur
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE ur.UserID = @UserID AND ur.TenantID = @TenantID";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, Role, UserRole>(
                sql,
                (userRole, role) =>
                {
                    userRole.Role = role;
                    return userRole;
                },
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "RoleID");

            return userRoles;
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId, int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ur.RoleID = @RoleID AND ur.TenantID = @TenantID";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, UserRole>(
                sql,
                (userRole, user) =>
                {
                    userRole.User = user;
                    return userRole;
                },
                new { RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userRoles;
        }

        public async Task<IEnumerable<UserRole>> GetActiveUserRolesAsync(int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email,
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE ur.IsActive = 1 AND ur.TenantID = @TenantID";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, Role, UserRole>(
                sql,
                (userRole, user, role) =>
                {
                    userRole.User = user;
                    userRole.Role = role;
                    return userRole;
                },
                new { TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId,RoleID");

            return userRoles;
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesByUserAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription
                FROM UserRoles ur
                INNER JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE ur.UserID = @UserID AND ur.TenantID = @TenantID AND ur.IsActive = 1";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, Role, UserRole>(
                sql,
                (userRole, role) =>
                {
                    userRole.Role = role;
                    return userRole;
                },
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "RoleID");

            return userRoles;
        }

        public async Task<IEnumerable<UserRole>> GetUsersInRoleAsync(int roleId, int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserRoles ur
                INNER JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ur.RoleID = @RoleID AND ur.TenantID = @TenantID AND ur.IsActive = 1";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, UserRole>(
                sql,
                (userRole, user) =>
                {
                    userRole.User = user;
                    return userRole;
                },
                new { RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userRoles;
        }

        public async Task<UserRole> GetByIdAsync(int id, int tenantId)
        {
            // Use the enhanced method that includes user and role details
            return await GetByIdWithDetailsAsync(id, tenantId);
        }

        public override async Task<int> InsertAsync(UserRole userRole)
        {
            const string sql = @"
                INSERT INTO UserRoles (
                    UserID, RoleID, TenantID, CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @UserID, @RoleID, @TenantID, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (userRole.CreatedDate == DateTime.MinValue)
            {
                userRole.CreatedDate = DateTime.UtcNow;
                userRole.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                userRole,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(UserRole userRole)
        {
            const string sql = @"
                UPDATE UserRoles
                SET UserID = @UserID,
                    RoleID = @RoleID,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE UserRoleID = @UserRoleID AND TenantID = @TenantID";

            // Set modification date
            userRole.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                userRole,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email,
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE ur.TenantID = @TenantID";

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, Role, UserRole>(
                sql,
                (userRole, user, role) =>
                {
                    userRole.User = user;
                    userRole.Role = role;
                    return userRole;
                },
                new { TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId,RoleID");

            return userRoles;
        }

        public async Task<PagedResult<UserRole>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "ur.TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    u.UserName LIKE @SearchTerm OR 
                    u.Email LIKE @SearchTerm OR
                    r.Name LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            // Custom query for paged results with joins
            string countSql = @"
                SELECT COUNT(*)
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE " + whereClause;

            string dataSql = @"
                SELECT ur.*, 
                       u.UserId, u.UserName, u.Email,
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription
                FROM UserRoles ur
                LEFT JOIN Users u ON ur.UserID = u.UserId AND u.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                WHERE " + whereClause + @"
                ORDER BY ur.UserRoleID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var totalCount = await _unitOfWork.Connection.QuerySingleAsync<int>(
                countSql.Replace(whereClause, whereClause),
                parameters,
                _unitOfWork.Transaction);

            var offset = (page - 1) * pageSize;
            var dataParameters = new DynamicParameters(parameters);
            dataParameters.Add("Offset", offset);
            dataParameters.Add("PageSize", pageSize);

            var userRoles = await _unitOfWork.Connection.QueryAsync<UserRole, User, Role, UserRole>(
                dataSql.Replace(whereClause, whereClause),
                (userRole, user, role) =>
                {
                    userRole.User = user;
                    userRole.Role = role;
                    return userRole;
                },
                dataParameters,
                _unitOfWork.Transaction,
                splitOn: "UserId,RoleID");

            return new PagedResult<UserRole>
            {
                Items = userRoles.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM UserRoles WHERE UserRoleID = @UserRoleID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserRoleID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId)
        {
            const string sql = @"
                DELETE FROM UserRoles 
                WHERE UserID = @UserID AND RoleID = @RoleID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserID = userId, RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UserIsInRoleAsync(int userId, int roleId, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserRoles 
                WHERE UserID = @UserID AND RoleID = @RoleID AND TenantID = @TenantID AND IsActive = 1";
            int count = await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { UserID = userId, RoleID = roleId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return count > 0;
        }

        public async Task<bool> DeactivateUserRoleAsync(int userRoleId, int tenantId)
        {
            const string sql = @"
                UPDATE UserRoles 
                SET IsActive = 0, ModifiedDate = @ModifiedDate
                WHERE UserRoleID = @UserRoleID AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserRoleID = userRoleId,
                    TenantID = tenantId,
                    ModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ActivateUserRoleAsync(int userRoleId, int tenantId)
        {
            const string sql = @"
                UPDATE UserRoles 
                SET IsActive = 1, ModifiedDate = @ModifiedDate
                WHERE UserRoleID = @UserRoleID AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserRoleID = userRoleId,
                    TenantID = tenantId,
                    ModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
