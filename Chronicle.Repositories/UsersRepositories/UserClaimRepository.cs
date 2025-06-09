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
    public class UserClaimRepository : DapperRepository<UserClaim, int>, IUserClaimRepository
    {
        public UserClaimRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "UserClaims", "UserClaimID")
        {
        }

        public async Task<UserClaim> GetByIdWithUserAsync(int id, int tenantId)
        {
            const string sql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName, u.IsActive as UserIsActive
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE uc.UserClaimID = @UserClaimID AND uc.TenantID = @TenantID";

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                sql,
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                new { UserClaimID = id, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userClaims.FirstOrDefault();
        }

        public async Task<IEnumerable<UserClaim>> GetByUserIdAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT * FROM UserClaims 
                WHERE UserID = @UserID AND TenantID = @TenantID
                ORDER BY ClaimType, ClaimValue";
            return await _unitOfWork.Connection.QueryAsync<UserClaim>(
                sql,
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<UserClaim>> GetByClaimTypeAsync(string claimType, int tenantId)
        {
            const string sql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE uc.ClaimType = @ClaimType AND uc.TenantID = @TenantID
                ORDER BY u.UserName, uc.ClaimValue";

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                sql,
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                new { ClaimType = claimType, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userClaims;
        }

        public async Task<UserClaim> GetByUserAndClaimAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            const string sql = @"
                SELECT * FROM UserClaims 
                WHERE UserID = @UserID AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserClaim>(
                sql,
                new { UserID = userId, ClaimType = claimType, ClaimValue = claimValue, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<UserClaim>> GetActiveUserClaimsAsync(int tenantId)
        {
            const string sql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE uc.IsActive = 1 AND uc.TenantID = @TenantID
                ORDER BY u.UserName, uc.ClaimType, uc.ClaimValue";

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                sql,
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                new { TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userClaims;
        }

        public async Task<UserClaim> GetByIdAsync(int id, int tenantId)
        {
            // Use the enhanced method that includes user details
            return await GetByIdWithUserAsync(id, tenantId);
        }

        public override async Task<int> InsertAsync(UserClaim userClaim)
        {
            const string sql = @"
                INSERT INTO UserClaims (
                    UserID, TenantID, ClaimType, ClaimValue, CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @UserID, @TenantID, @ClaimType, @ClaimValue, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (userClaim.CreatedDate == DateTime.MinValue)
            {
                userClaim.CreatedDate = DateTime.UtcNow;
                userClaim.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                userClaim,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(UserClaim userClaim)
        {
            const string sql = @"
                UPDATE UserClaims
                SET UserID = @UserID,
                    ClaimType = @ClaimType,
                    ClaimValue = @ClaimValue,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE UserClaimID = @UserClaimID AND TenantID = @TenantID";

            // Set modification date
            userClaim.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                userClaim,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<UserClaim>> GetAllAsync(int tenantId)
        {
            const string sql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE uc.TenantID = @TenantID
                ORDER BY u.UserName, uc.ClaimType, uc.ClaimValue";

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                sql,
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                new { TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userClaims;
        }

        public async Task<PagedResult<UserClaim>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "uc.TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    u.UserName LIKE @SearchTerm OR 
                    u.Email LIKE @SearchTerm OR
                    uc.ClaimType LIKE @SearchTerm OR
                    uc.ClaimValue LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            // Custom query for paged results with joins
            string countSql = @"
                SELECT COUNT(*)
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE " + whereClause;

            string dataSql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE " + whereClause + @"
                ORDER BY u.UserName, uc.ClaimType, uc.ClaimValue
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var totalCount = await _unitOfWork.Connection.QuerySingleAsync<int>(
                countSql.Replace(whereClause, whereClause),
                parameters,
                _unitOfWork.Transaction);

            var offset = (page - 1) * pageSize;
            var dataParameters = new DynamicParameters(parameters);
            dataParameters.Add("Offset", offset);
            dataParameters.Add("PageSize", pageSize);

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                dataSql.Replace(whereClause, whereClause),
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                dataParameters,
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return new PagedResult<UserClaim>
            {
                Items = userClaims.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            const string sql = "DELETE FROM UserClaims WHERE UserClaimID = @UserClaimID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserClaimID = id, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByUserIdAsync(int userId, int tenantId)
        {
            const string sql = "DELETE FROM UserClaims WHERE UserID = @UserID AND TenantID = @TenantID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ClaimExistsForUserAsync(int userId, string claimType, string claimValue, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserClaims 
                WHERE UserID = @UserID AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue AND TenantID = @TenantID";
            int count = await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { UserID = userId, ClaimType = claimType, ClaimValue = claimValue, TenantID = tenantId },
                _unitOfWork.Transaction);

            return count > 0;
        }

        public async Task<IEnumerable<string>> GetClaimTypesAsync(int tenantId)
        {
            const string sql = @"
                SELECT DISTINCT ClaimType 
                FROM UserClaims 
                WHERE TenantID = @TenantID 
                ORDER BY ClaimType";
            return await _unitOfWork.Connection.QueryAsync<string>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<UserClaim>> GetClaimsByTypeAsync(string claimType, int tenantId)
        {
            const string sql = @"
                SELECT uc.*, 
                       u.UserId, u.UserName, u.Email, u.FirstName, u.LastName
                FROM UserClaims uc
                LEFT JOIN Users u ON uc.UserID = u.UserId AND u.TenantID = @TenantID
                WHERE uc.ClaimType = @ClaimType AND uc.TenantID = @TenantID
                ORDER BY u.UserName, uc.ClaimValue";

            var userClaims = await _unitOfWork.Connection.QueryAsync<UserClaim, User, UserClaim>(
                sql,
                (userClaim, user) =>
                {
                    userClaim.User = user;
                    return userClaim;
                },
                new { ClaimType = claimType, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "UserId");

            return userClaims;
        }

        public async Task<bool> DeactivateUserClaimAsync(int userClaimId, int tenantId)
        {
            const string sql = @"
                UPDATE UserClaims 
                SET IsActive = 0, ModifiedDate = @ModifiedDate
                WHERE UserClaimID = @UserClaimID AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserClaimID = userClaimId,
                    TenantID = tenantId,
                    ModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ActivateUserClaimAsync(int userClaimId, int tenantId)
        {
            const string sql = @"
                UPDATE UserClaims 
                SET IsActive = 1, ModifiedDate = @ModifiedDate
                WHERE UserClaimID = @UserClaimID AND TenantID = @TenantID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new
                {
                    UserClaimID = userClaimId,
                    TenantID = tenantId,
                    ModifiedDate = DateTime.UtcNow
                },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<int> GetUserClaimCountAsync(int userId, int tenantId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserClaims 
                WHERE UserID = @UserID AND TenantID = @TenantID AND IsActive = 1";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { UserID = userId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }
    }
}
