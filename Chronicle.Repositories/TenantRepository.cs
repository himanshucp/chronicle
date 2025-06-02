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
    public class TenantRepository : DapperRepository<Tenant, int>, ITenantRepository
    {
        public TenantRepository(IUnitOfWork unitOfWork, string tableName, string idColumn) : base(unitOfWork, tableName, idColumn)
        {
        }

        public override async Task<int> InsertAsync(Tenant tenant)
        {
            const string sql = @"
                INSERT INTO Users (
                     TenantName, TenantCode, ContactEmail, ContactPhone, Address,
                        SubscriptionLevel, MaxUsers, MaxProjects, CreatedDate, ModifiedDate, IsActive
                )
                VALUES (
                     @TenantName, @TenantCode, @ContactEmail, @ContactPhone, @Address,
                        @SubscriptionLevel, @MaxUsers, @MaxProjects, @CreatedDate, @ModifiedDate, @IsActive
                );
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (tenant.CreatedDate == default)
            {
                tenant.CreatedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                tenant,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Tenant tenant)
        {
            const string sql = @"
               SET TenantName = @TenantName,
                        ContactEmail = @ContactEmail,
                        ContactPhone = @ContactPhone,
                        Address = @Address,
                        SubscriptionLevel = @SubscriptionLevel,
                        MaxUsers = @MaxUsers,
                        MaxProjects = @MaxProjects,
                        ModifiedDate = GETDATE()
                    WHERE TenantID = @TenantID";

            // Set modification date
            //tenant.LastModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                tenant,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<Tenant> GetByCodeAsync(string tenantCode)
        {
            const string sql = "SELECT * FROM Tenants WHERE TenantCode = @TenantCode";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Tenant>(
                sql,
                new { TenantCode = tenantCode },
                _unitOfWork.Transaction);
        }

        public async Task<bool> IsTenantCodeUniqueAsync(string tenantCode, int? excludeTenantId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsTenantNameUniqueAsync(string tenantName, int? excludeTenantId)
        {
            throw new NotImplementedException();
        }
    }
}
