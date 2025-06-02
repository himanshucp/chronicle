using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Data.Extensions
{
    /// <summary>
    /// Extension methods for Dapper
    /// </summary>
    public static class DapperExtensions
    {
        /// <summary>
        /// Executes a paged query
        /// </summary>
        public static async Task<PagedResult<T>> QueryPagedAsync<T>(
            this IDbConnection connection,
            string tableName,
            string orderByColumn,
            int page,
            int pageSize,
            string whereClause = null,
            object parameters = null,
            IDbTransaction transaction = null)
        {
            whereClause = string.IsNullOrEmpty(whereClause) ? "" : $"WHERE {whereClause}";

            string countSql = $"SELECT COUNT(1) FROM {tableName} {whereClause}";

            string pagingSql = $@"
                SELECT *
                FROM {tableName}
                {whereClause}
                ORDER BY {orderByColumn}
                OFFSET {(page - 1) * pageSize} ROWS
                FETCH NEXT {pageSize} ROWS ONLY";

            int totalCount = await connection.ExecuteScalarAsync<int>(
                countSql,
                parameters,
                transaction);

            IEnumerable<T> items = await connection.QueryAsync<T>(
                pagingSql,
                parameters,
                transaction);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Executes a multi-mapping query for parent and children entities
        /// </summary>
        public static async Task<IEnumerable<TParent>> QueryParentChildAsync<TParent, TChild, TKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TKey> parentKeySelector,
            Func<TChild, TKey> childKeySelector,
            Func<TParent, ICollection<TChild>> childSelector,
            object parameters = null,
            IDbTransaction transaction = null,
            string splitOn = "Id")
            where TParent : class
            where TChild : class
        {
            var lookup = new Dictionary<TKey, TParent>();

            await connection.QueryAsync<TParent, TChild, TParent>(
                sql,
                (parent, child) =>
                {
                    var key = parentKeySelector(parent);

                    if (!lookup.TryGetValue(key, out TParent existingParent))
                    {
                        existingParent = parent;
                        childSelector(existingParent).Clear();
                        lookup.Add(key, existingParent);
                    }

                    if (child != null)
                    {
                        childSelector(existingParent).Add(child);
                    }

                    return existingParent;
                },
                parameters,
                transaction,
                splitOn: splitOn);

            return lookup.Values;
        }

        /// <summary>
        /// Executes a bulk insert operation
        /// </summary>
        public static async Task BulkInsertAsync<T>(
            this IDbConnection connection,
            IEnumerable<T> entities,
            string tableName,
            int batchSize = 1000,
            IDbTransaction transaction = null)
        {
            if (entities == null || !entities.Any())
                return;

            // Get property names except Id (for identity insert)
            var properties = typeof(T).GetProperties()
                .Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && p.CanWrite)
                .ToArray();

            var columnNames = string.Join(", ", properties.Select(p => p.Name));
            var paramNames = string.Join(", ", properties.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";

            // Split into batches for better performance
            var entityList = entities.ToList();
            for (int i = 0; i < entityList.Count; i += batchSize)
            {
                var batch = entityList.Skip(i).Take(batchSize).ToList();
                await connection.ExecuteAsync(sql, batch, transaction);
            }
        }
    }
}
