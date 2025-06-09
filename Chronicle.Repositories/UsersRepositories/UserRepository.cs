using Chronicle.Data.Extensions;
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
    public class UserRepository : DapperRepository<User, int>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Users", "UserId")
        {
        }

        public async Task<User> GetByIdWithRolesAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT u.*, 
                       ur.UserRoleID, ur.UserID as URUserID, ur.RoleID, ur.TenantID as URTenantID,
                       r.RoleID, r.Name as RoleName, r.Description as RoleDescription,
                       ul.UserLoginID, ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName,
                       uc.UserClaimID, uc.ClaimType, uc.ClaimValue
                FROM Users u
                LEFT JOIN UserRoles ur ON u.UserId = ur.UserID AND ur.TenantID = @TenantID
                LEFT JOIN Roles r ON ur.RoleID = r.RoleID AND r.TenantID = @TenantID
                LEFT JOIN UserLogins ul ON u.UserId = ul.UserID AND ul.TenantID = @TenantID
                LEFT JOIN UserClaims uc ON u.UserId = uc.UserID AND uc.TenantID = @TenantID
                WHERE u.UserId = @UserId AND u.TenantID = @TenantID";

            var userDictionary = new Dictionary<int, User>();

            var users = await _unitOfWork.Connection.QueryAsync<User, UserRole, Role, UserLogin, UserClaim, User>(
                sql,
                (user, userRole, role, userLogin, userClaim) =>
                {
                    if (!userDictionary.TryGetValue(user.UserId, out User userEntry))
                    {
                        userEntry = user;
                        userEntry.UserRoles = new List<UserRole>();
                        userEntry.UserLogins = new List<UserLogin>();
                        userEntry.UserClaims = new List<UserClaim>();
                        userDictionary.Add(user.UserId, userEntry);
                    }

                    if (userRole != null && userRole.UserRoleID > 0)
                    {
                        var existingUserRole = userEntry.UserRoles
                            .FirstOrDefault(ur => ur.UserRoleID == userRole.UserRoleID);

                        if (existingUserRole == null)
                        {
                            userRole.Role = role;
                            userEntry.UserRoles.Add(userRole);
                        }
                    }

                    if (userLogin != null && userLogin.UserLoginID > 0)
                    {
                        var existingUserLogin = userEntry.UserLogins
                            .FirstOrDefault(ul => ul.UserLoginID == userLogin.UserLoginID);

                        if (existingUserLogin == null)
                        {
                            userEntry.UserLogins.Add(userLogin);
                        }
                    }

                    if (userClaim != null && userClaim.UserClaimID > 0)
                    {
                        var existingUserClaim = userEntry.UserClaims
                            .FirstOrDefault(uc => uc.UserClaimID == userClaim.UserClaimID);

                        if (existingUserClaim == null)
                        {
                            userEntry.UserClaims.Add(userClaim);
                        }
                    }

                    return userEntry;
                },
                new { UserId = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserRoleID,RoleID,UserLoginID,UserClaimID");

            return users.FirstOrDefault();
        }

        public async Task<User> GetByUserNameAsync(string userName, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE UserName = @UserName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { UserName = userName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<User> GetByEmailAsync(string email, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE Email = @Email AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Email = email, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<User> GetByEmployeeIdAsync(int employeeId, int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE EmployeeID = @EmployeeID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { EmployeeID = employeeId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<User>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, int tenantId)
        {
            const string sql = @"
                SELECT DISTINCT u.*
                FROM Users u
                INNER JOIN UserRoles ur ON u.UserId = ur.UserID
                INNER JOIN Roles r ON ur.RoleID = r.RoleID
                WHERE r.Name = @RoleName AND u.TenantID = @TenantID AND r.TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<User>(
                sql,
                new { RoleName = roleName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<User> GetByIdAsync(int id, int tenantId)
        {
            // Use the enhanced method that includes roles
            return await GetByIdWithRolesAsync(id, tenantId);
        }

        public override async Task<int> InsertAsync(User user)
        {
            const string sql = @"
                INSERT INTO Users (
                    TenantID, EmployeeID, UserName, NormalizedUserName, Email, NormalizedEmail, 
                    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, 
                    PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, 
                    AccessFailedCount, LastLoginDate, IsActive, CreatedDate, LastModifiedDate)
                VALUES (
                    @TenantID, @EmployeeID, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, 
                    @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, 
                    @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, 
                    @AccessFailedCount, @LastLoginDate, @IsActive, @CreatedDate, @LastModifiedDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (user.CreatedDate == DateTime.MinValue)
            {
                user.CreatedDate = DateTime.UtcNow;
                user.LastModifiedDate = DateTime.UtcNow;
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
                SET EmployeeID = @EmployeeID,
                    UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    EmailConfirmed = @EmailConfirmed,
                    PasswordHash = @PasswordHash,
                    SecurityStamp = @SecurityStamp,
                    ConcurrencyStamp = @ConcurrencyStamp,
                    PhoneNumber = @PhoneNumber,
                    PhoneNumberConfirmed = @PhoneNumberConfirmed,
                    TwoFactorEnabled = @TwoFactorEnabled,
                    LockoutEnd = @LockoutEnd,
                    LockoutEnabled = @LockoutEnabled,
                    AccessFailedCount = @AccessFailedCount,
                    LastLoginDate = @LastLoginDate,
                    IsActive = @IsActive,
                    LastModifiedDate = @LastModifiedDate
                WHERE UserId = @UserId AND TenantID = @TenantID";

            // Set modification date
            user.LastModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                user,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Users WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<User>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    UserName LIKE @SearchTerm OR 
                    Email LIKE @SearchTerm OR
                    PhoneNumber LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<User>(
                "Users",
                "UserName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM Users WHERE UserId = @UserId AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserId = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId, int tenantId)
        {
            const string sql = @"
                UPDATE Users 
                SET LastLoginDate = @LastLoginDate, LastModifiedDate = @LastModifiedDate
                WHERE UserId = @UserId AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    TenantID = tenantId,
                    LastLoginDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> IncrementAccessFailedCountAsync(int userId, int tenantId)
        {
            const string sql = @"
                UPDATE Users 
                SET AccessFailedCount = AccessFailedCount + 1, LastModifiedDate = @LastModifiedDate
                WHERE UserId = @UserId AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    TenantID = tenantId,
                    LastModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ResetAccessFailedCountAsync(int userId, int tenantId)
        {
            const string sql = @"
                UPDATE Users 
                SET AccessFailedCount = 0, LastModifiedDate = @LastModifiedDate
                WHERE UserId = @UserId AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    TenantID = tenantId,
                    LastModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> SetLockoutAsync(int userId, DateTimeOffset? lockoutEnd, int tenantId)
        {
            const string sql = @"
                UPDATE Users 
                SET LockoutEnd = @LockoutEnd, LastModifiedDate = @LastModifiedDate
                WHERE UserId = @UserId AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    TenantID = tenantId,
                    LockoutEnd = lockoutEnd,
                    LastModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
