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
    public class UserLoginRepository : DapperRepository<UserLogin, int>, IUserLoginRepository
    {
        public UserLoginRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "UserLogins", "UserLoginID")
        {
        }

        public async Task<UserLogin> GetByIdWithUserAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT ul.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName, u.IsActive as UserIsActive
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ul.UserLoginID = @UserLoginID AND ul.TenantID = @TenantID";

            var userLogins = await _unitOfWork.Connection.QueryAsync<UserLogin, User, UserLogin>(
                sql,
                (userLogin, user) =>
                {
                    userLogin.User = user;
                    return userLogin;
                },
                new { UserLoginID = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userLogins.FirstOrDefault();
        }

        public async Task<UserLogin> GetByProviderAsync(string loginProvider, string providerKey, int tenantId)
        {
            const string sql = @"
                SELECT ul.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ul.LoginProvider = @LoginProvider AND ul.ProviderKey = @ProviderKey AND ul.TenantID = @TenantID";

            var userLogins = await _unitOfWork.Connection.QueryAsync<UserLogin, User, UserLogin>(
                sql,
                (userLogin, user) =>
                {
                    userLogin.User = user;
                    return userLogin;
                },
                new { LoginProvider = loginProvider, ProviderKey = providerKey, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userLogins.FirstOrDefault();
        }

        public async Task<IEnumerable<UserLogin>> GetByUserIdAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT * FROM UserLogins 
                WHERE UserID = @UserID AND TenantID = @TenantID AND IsActive = 1
                ORDER BY LoginProvider";
            return await _unitOfWork.Connection.QueryAsync<UserLogin>(
                sql,
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<UserLogin>> GetByLoginProviderAsync(string loginProvider, int tenantId)
        {
            const string sql = @"
                SELECT ul.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ul.LoginProvider = @LoginProvider AND ul.TenantID = @TenantID AND ul.IsActive = 1
                ORDER BY u.UserName";

            var userLogins = await _unitOfWork.Connection.QueryAsync<UserLogin, User, UserLogin>(
                sql,
                (userLogin, user) =>
                {
                    userLogin.User = user;
                    return userLogin;
                },
                new { LoginProvider = loginProvider, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userLogins;
        }

        public async Task<UserLogin> GetUserLoginAsync(int userId, string loginProvider, int tenantId)
        {
            const string sql = @"
                SELECT * FROM UserLogins 
                WHERE UserID = @UserID AND LoginProvider = @LoginProvider AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserLogin>(
                sql,
                new { UserID = userId, LoginProvider = loginProvider, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<UserLogin> GetByIdAsync(int id, int tenantId)
        {
            // Use the enhanced method that includes user details
            return await GetByIdWithUserAsync(id, tenantId);
        }

        public override async Task<int> InsertAsync(UserLogin userLogin)
        {
            const string sql = @"
                INSERT INTO UserLogins (
                    UserID, TenantID, LoginProvider, ProviderKey, ProviderDisplayName, 
                    CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @UserID, @TenantID, @LoginProvider, @ProviderKey, @ProviderDisplayName, 
                    @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (userLogin.CreatedDate == DateTime.MinValue)
            {
                userLogin.CreatedDate = DateTime.UtcNow;
                userLogin.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                userLogin,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(UserLogin userLogin)
        {
            const string sql = @"
                UPDATE UserLogins
                SET UserID = @UserID,
                    LoginProvider = @LoginProvider,
                    ProviderKey = @ProviderKey,
                    ProviderDisplayName = @ProviderDisplayName,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE UserLoginID = @UserLoginID AND TenantID = @TenantID";

            // Set modification date
            userLogin.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                userLogin,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<UserLogin>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT ul.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE ul.TenantID = @TenantID
                ORDER BY u.UserName, ul.LoginProvider";

            var userLogins = await _unitOfWork.Connection.QueryAsync<UserLogin, User, UserLogin>(
                sql,
                (userLogin, user) =>
                {
                    userLogin.User = user;
                    return userLogin;
                },
                new { TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userLogins;
        }

        public async Task<PagedResult<UserLogin>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "ul.TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    u.UserName LIKE @SearchTerm OR 
                    u.Email LIKE @SearchTerm OR
                    ul.LoginProvider LIKE @SearchTerm OR
                    ul.ProviderDisplayName LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            // Custom query for paged results with joins
            string countSql = @"
                SELECT COUNT(*)
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE " + whereClause;

            string dataSql = @"
                SELECT ul.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserLogins ul
                LEFT JOIN Users u ON ul.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE " + whereClause + @"
                ORDER BY u.UserName, ul.LoginProvider
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var totalCount = await _unitOfWork.Connection.QuerySingleAsync<int>(
                countSql.Replace(whereClause, whereClause),
                parameters,
                _unitOfWork.Transaction);

            var offset = (page - 1) * pageSize;
            var dataParameters = new DynamicParameters(parameters);
            dataParameters.Add("Offset", offset);
            dataParameters.Add("PageSize", pageSize);

            var userLogins = await _unitOfWork.Connection.QueryAsync<UserLogin, User, UserLogin>(
                dataSql.Replace(whereClause, whereClause),
                (userLogin, user) =>
                {
                    userLogin.User = user;
                    return userLogin;
                },
                dataParameters,
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return new PagedResult<UserLogin>
            {
                Items = userLogins.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM UserLogins WHERE UserLoginID = @UserLoginID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserLoginID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByUserAndProviderAsync(int userId, string loginProvider, int tenantId)
        {
            const string sql = @"
                DELETE FROM UserLogins 
                WHERE UserID = @UserID AND LoginProvider = @LoginProvider AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserID = userId, LoginProvider = loginProvider, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string loginProvider, string providerKey, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserLogins 
                WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey AND TenantID = @TenantID";
            int count = await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { LoginProvider = loginProvider, ProviderKey = providerKey, TenantID = tenantId },
                _unitOfWork.Transaction);

            return count > 0;
        }

        public async Task<User> FindUserByLoginAsync(string loginProvider, string providerKey, int tenantId)
        {
            const string sql = @"
                SELECT u.*
                FROM Users u
                INNER JOIN UserLogins ul ON u.UserId = ul.UserID
                WHERE ul.LoginProvider = @LoginProvider AND ul.ProviderKey = @ProviderKey 
                      AND ul.TenantID = @TenantID AND u.TenantID = @TenantID AND ul.IsActive = 1";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { LoginProvider = loginProvider, ProviderKey = providerKey, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<string>> GetLoginProvidersAsync(int tenantId)
        {
            const string sql = @"
                SELECT DISTINCT LoginProvider 
                FROM UserLogins 
                WHERE TenantID = @TenantID AND IsActive = 1
                ORDER BY LoginProvider";
            return await _unitOfWork.Connection.QueryAsync<string>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<int> GetUserLoginCountAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserLogins 
                WHERE UserID = @UserID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }
    }
}
