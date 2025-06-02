using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Data.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Chronicle.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    public class UserRepository : DapperRepository<User, int>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Users", "UserId")
        {
        }

        public async Task<User> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE UserId = @Id AND TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Id = id, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<User>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryAsync<User>(
                sql,
                new { TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(User user)
        {
            const string sql = @"
            INSERT INTO Users (
                Username, Email, PasswordHash, SecurityStamp,
                PhoneNumber, PhoneNumberConfirmed, EmailConfirmed,
                TwoFactorEnabled, LockoutEnd, LockoutEnabled,
                AccessFailedCount, IsActive, CreatedDate, LastModifiedDate,TenantID
            )
            VALUES (
                @Username, @Email, @PasswordHash, @SecurityStamp,
                @PhoneNumber, @PhoneNumberConfirmed, @EmailConfirmed,
                @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled,
                @AccessFailedCount, @IsActive, @CreatedDate, @LastModifiedDate,@TenantID
            );
            SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (user.CreatedDate == default)
            {
                user.CreatedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                user,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(User user)
        {
            const string sql = @"
            UPDATE Users 
            SET Username = @Username,
                Email = @Email,
                PasswordHash = @PasswordHash,
                SecurityStamp = @SecurityStamp,
                PhoneNumber = @PhoneNumber,
                PhoneNumberConfirmed = @PhoneNumberConfirmed,
                EmailConfirmed = @EmailConfirmed,
                TwoFactorEnabled = @TwoFactorEnabled,
                LockoutEnd = @LockoutEnd,
                LockoutEnabled = @LockoutEnabled,
                AccessFailedCount = @AccessFailedCount,
                IsActive = @IsActive,
                LastModifiedDate = @LastModifiedDate
            WHERE UserId = @UserId 
            AND TenantID = @TenantID";

            // Set modification date
            user.LastModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                user,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM Users WHERE UserId = @Id AND TenantID = @TenantId";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { Id = id, TenantId = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<User> GetByUsernameAsync(string username, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE Username = @Username AND TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Username = username, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<User> GetByEmailAsync(string email, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE Email = @Email AND TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Email = email, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<UserWithRoles> GetUserWithRolesAsync(int userId, int tenantId)
        {
            const string sql = @"
            SELECT u.*, r.*
            FROM Users u
            LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
            LEFT JOIN Roles r ON ur.RoleId = r.RoleId
            WHERE u.UserId = @UserId AND u.TenantID = @TenantId";

            UserWithRoles result = null;
            var roles = new List<Role>();

            await _unitOfWork.Connection.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    if (result == null)
                    {
                        result = new UserWithRoles
                        {
                            User = user,
                            Roles = roles
                        };
                    }

                    if (role != null)
                    {
                        roles.Add(role);
                    }

                    return user;
                },
                new { UserId = userId, TenantId = tenantId },
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId, int tenantId)
        {
            const string sql = @"
            SELECT u.*
            FROM Users u
            JOIN UserRoles ur ON u.UserId = ur.UserId
            WHERE ur.RoleId = @RoleId AND u.TenantID = @TenantId";

            return await _unitOfWork.Connection.QueryAsync<User>(
                sql,
                new { RoleId = roleId, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<bool> AddUserToRoleAsync(int userId, int roleId, int tenantId)
        {
            const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId AND TenantID = @TenantId)
            BEGIN
                INSERT INTO UserRoles (UserId, RoleId, TenantID)
                VALUES (@UserId, @RoleId, @TenantId)
            END";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserId = userId, RoleId = roleId, TenantId = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveUserFromRoleAsync(int userId, int roleId, int tenantId)
        {
            const string sql = @"
            DELETE FROM UserRoles 
            WHERE UserId = @UserId AND RoleId = @RoleId AND TenantID = @TenantId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserId = userId, RoleId = roleId, TenantId = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, int tenantId)
        {
            try
            {
                // Delete existing roles
                const string deleteRoles = @"
                DELETE FROM UserRoles WHERE UserId = @UserId AND TenantID = @TenantId";

                await _unitOfWork.Connection.ExecuteAsync(
                    deleteRoles,
                    new { UserId = userId, TenantId = tenantId },
                    _unitOfWork.Transaction);

                // Add new roles
                if (roleIds != null && roleIds.Any())
                {
                    const string addRoles = @"
                    INSERT INTO UserRoles (UserId, RoleId, TenantID)
                    VALUES (@UserId, @RoleId, @TenantId)";

                    var parameters = roleIds.Select(roleId =>
                        new { UserId = userId, RoleId = roleId, TenantId = tenantId }).ToList();

                    await _unitOfWork.Connection.ExecuteAsync(
                        addRoles,
                        parameters,
                        _unitOfWork.Transaction);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantId";
            object parameters = new { TenantId = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                Username LIKE @SearchTerm OR 
                Email LIKE @SearchTerm OR 
                PhoneNumber LIKE @SearchTerm)";

                parameters = new
                {
                    TenantId = tenantId,
                    SearchTerm = $"%{searchTerm}%"
                };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<User>(
                "Users",
                "Username",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }
    }
}
