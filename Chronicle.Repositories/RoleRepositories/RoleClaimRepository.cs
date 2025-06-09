using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories.RoleRepositories
{
    public class RoleClaimRepository : DapperRepository<RoleClaim, int>, IRoleClaimRepository
    {
        public RoleClaimRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "RoleClaims", "RoleClaimId")
        {
        }

        public async Task<RoleClaim> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT rc.*, r.RoleId, r.Name as RoleName, r.Description as RoleDescription
                FROM RoleClaims rc
                LEFT JOIN Roles r ON rc.RoleId = r.RoleId
                WHERE rc.RoleClaimId = @RoleClaimId";

            var roleClaims = await _unitOfWork.Connection.QueryAsync<RoleClaim, Role, RoleClaim>(
                sql,
                (roleClaim, role) =>
                {
                    roleClaim.Role = role;
                    return roleClaim;
                },
                new { RoleClaimId = id },
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return roleClaims.FirstOrDefault();
        }

        public async Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(int roleId)
        {
            const string sql = "SELECT * FROM RoleClaims WHERE RoleId = @RoleId ORDER BY ClaimType, ClaimValue";
            return await _unitOfWork.Connection.QueryAsync<RoleClaim>(
                sql,
                new { RoleId = roleId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<RoleClaim>> GetByClaimTypeAsync(string claimType)
        {
            const string sql = @"
                SELECT rc.*, r.RoleId, r.Name as RoleName, r.Description as RoleDescription
                FROM RoleClaims rc
                LEFT JOIN Roles r ON rc.RoleId = r.RoleId
                WHERE rc.ClaimType = @ClaimType
                ORDER BY r.Name, rc.ClaimValue";

            var roleClaims = await _unitOfWork.Connection.QueryAsync<RoleClaim, Role, RoleClaim>(
                sql,
                (roleClaim, role) =>
                {
                    roleClaim.Role = role;
                    return roleClaim;
                },
                new { ClaimType = claimType },
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return roleClaims;
        }

        public async Task<RoleClaim> GetByRoleAndClaimAsync(int roleId, string claimType, string claimValue)
        {
            const string sql = @"
                SELECT * FROM RoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<RoleClaim>(
                sql,
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<RoleClaim>> GetClaimsByRoleNameAsync(string roleName, int tenantId)
        {
            const string sql = @"
                SELECT rc.*, r.RoleId, r.Name as RoleName, r.Description as RoleDescription
                FROM RoleClaims rc
                INNER JOIN Roles r ON rc.RoleId = r.RoleId
                WHERE r.Name = @RoleName AND r.TenantID = @TenantID
                ORDER BY rc.ClaimType, rc.ClaimValue";

            var roleClaims = await _unitOfWork.Connection.QueryAsync<RoleClaim, Role, RoleClaim>(
                sql,
                (roleClaim, role) =>
                {
                    roleClaim.Role = role;
                    return roleClaim;
                },
                new { RoleName = roleName, TenantID = tenantId },
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return roleClaims;
        }

        public override async Task<int> InsertAsync(RoleClaim roleClaim)
        {
            const string sql = @"
                INSERT INTO RoleClaims (RoleId, ClaimType, ClaimValue)
                VALUES (@RoleId, @ClaimType, @ClaimValue);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                roleClaim,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(RoleClaim roleClaim)
        {
            const string sql = @"
                UPDATE RoleClaims
                SET RoleId = @RoleId,
                    ClaimType = @ClaimType,
                    ClaimValue = @ClaimValue
                WHERE RoleClaimId = @RoleClaimId";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                roleClaim,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<RoleClaim>> GetAllAsync()
        {
            const string sql = @"
                SELECT rc.*, r.RoleId, r.Name as RoleName, r.Description as RoleDescription
                FROM RoleClaims rc
                LEFT JOIN Roles r ON rc.RoleId = r.RoleId
                ORDER BY r.Name, rc.ClaimType, rc.ClaimValue";

            var roleClaims = await _unitOfWork.Connection.QueryAsync<RoleClaim, Role, RoleClaim>(
                sql,
                (roleClaim, role) =>
                {
                    roleClaim.Role = role;
                    return roleClaim;
                },
                null,
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return roleClaims;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM RoleClaims WHERE RoleClaimId = @RoleClaimId";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { RoleClaimId = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByRoleIdAsync(int roleId)
        {
            const string sql = "DELETE FROM RoleClaims WHERE RoleId = @RoleId";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { RoleId = roleId },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ClaimExistsForRoleAsync(int roleId, string claimType, string claimValue)
        {
            const string sql = @"
                SELECT COUNT(*) FROM RoleClaims 
                WHERE RoleId = @RoleId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";
            int count = await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                new { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue },
                _unitOfWork.Transaction);

            return count > 0;
        }

        public async Task<IEnumerable<string>> GetClaimTypesAsync()
        {
            const string sql = "SELECT DISTINCT ClaimType FROM RoleClaims ORDER BY ClaimType";
            return await _unitOfWork.Connection.QueryAsync<string>(
                sql,
                null,
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<RoleClaim>> GetClaimsByTypeAsync(string claimType)
        {
            const string sql = @"
                SELECT rc.*, r.RoleId, r.Name as RoleName, r.Description as RoleDescription
                FROM RoleClaims rc
                LEFT JOIN Roles r ON rc.RoleId = r.RoleId
                WHERE rc.ClaimType = @ClaimType
                ORDER BY r.Name, rc.ClaimValue";

            var roleClaims = await _unitOfWork.Connection.QueryAsync<RoleClaim, Role, RoleClaim>(
                sql,
                (roleClaim, role) =>
                {
                    roleClaim.Role = role;
                    return roleClaim;
                },
                new { ClaimType = claimType },
                _unitOfWork.Transaction,
                splitOn: "RoleId");

            return roleClaims;
        }
    }
}
