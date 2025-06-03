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
    public class InspectionMetricRepository : DapperRepository<InspectionMetric, int>, IInspectionMetricRepository
    {
        public InspectionMetricRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "InspectionMetrics", "MetricID")
        {
        }

        public async Task<IEnumerable<InspectionMetric>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE InspectionRequestID = @InspectionRequestID AND IsActive = 1 ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetByMetricTypeAsync(string metricType)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE MetricType = @MetricType AND IsActive = 1 ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { MetricType = metricType },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetByMetricNameAsync(string metricName)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE MetricName = @MetricName AND IsActive = 1 ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { MetricName = metricName },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetByCalculatedByAsync(int calculatedBy)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE CalculatedBy = @CalculatedBy AND IsActive = 1 ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { CalculatedBy = calculatedBy },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetByCalculationDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE CalculationDate >= @FromDate AND CalculationDate <= @ToDate AND IsActive = 1 ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetMetricsAboveTargetAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE TargetValue IS NOT NULL 
                AND MetricValue > TargetValue 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetMetricsBelowTargetAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE TargetValue IS NOT NULL 
                AND MetricValue < TargetValue 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetMetricsWithinVarianceAsync(decimal varianceThreshold)
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE Variance IS NOT NULL 
                AND ABS(Variance) <= @VarianceThreshold 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { VarianceThreshold = varianceThreshold },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetMetricsAboveBenchmarkAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE BenchmarkValue IS NOT NULL 
                AND MetricValue > BenchmarkValue 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<InspectionMetric>> GetMetricsBelowBenchmarkAsync()
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE BenchmarkValue IS NOT NULL 
                AND MetricValue < BenchmarkValue 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<decimal> GetAverageMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            string sql;
            object parameters;

            if (fromDate.HasValue && toDate.HasValue)
            {
                sql = "SELECT AVG(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND CalculationDate >= @FromDate AND CalculationDate <= @ToDate AND IsActive = 1";
                parameters = new { MetricName = metricName, FromDate = fromDate.Value, ToDate = toDate.Value };
            }
            else
            {
                sql = "SELECT AVG(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND IsActive = 1";
                parameters = new { MetricName = metricName };
            }

            var result = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<decimal?>(
                sql,
                parameters,
                _unitOfWork.Transaction);

            return result ?? 0;
        }

        public async Task<decimal> GetMaxMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            string sql;
            object parameters;

            if (fromDate.HasValue && toDate.HasValue)
            {
                sql = "SELECT MAX(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND CalculationDate >= @FromDate AND CalculationDate <= @ToDate AND IsActive = 1";
                parameters = new { MetricName = metricName, FromDate = fromDate.Value, ToDate = toDate.Value };
            }
            else
            {
                sql = "SELECT MAX(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND IsActive = 1";
                parameters = new { MetricName = metricName };
            }

            var result = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<decimal?>(
                sql,
                parameters,
                _unitOfWork.Transaction);

            return result ?? 0;
        }

        public async Task<decimal> GetMinMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            string sql;
            object parameters;

            if (fromDate.HasValue && toDate.HasValue)
            {
                sql = "SELECT MIN(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND CalculationDate >= @FromDate AND CalculationDate <= @ToDate AND IsActive = 1";
                parameters = new { MetricName = metricName, FromDate = fromDate.Value, ToDate = toDate.Value };
            }
            else
            {
                sql = "SELECT MIN(MetricValue) FROM InspectionMetrics WHERE MetricName = @MetricName AND IsActive = 1";
                parameters = new { MetricName = metricName };
            }

            var result = await _unitOfWork.Connection.QuerySingleOrDefaultAsync<decimal?>(
                sql,
                parameters,
                _unitOfWork.Transaction);

            return result ?? 0;
        }

        public async Task<IEnumerable<InspectionMetric>> GetTrendDataAsync(string metricName, DateTime fromDate, DateTime toDate)
        {
            const string sql = @"
                SELECT * FROM InspectionMetrics 
                WHERE MetricName = @MetricName 
                AND CalculationDate >= @FromDate 
                AND CalculationDate <= @ToDate 
                AND IsActive = 1 
                ORDER BY CalculationDate ASC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { MetricName = metricName, FromDate = fromDate, ToDate = toDate },
                _unitOfWork.Transaction);
        }

        public async Task<Dictionary<string, decimal>> GetMetricSummaryAsync(int inspectionRequestId)
        {
            const string sql = @"
                SELECT MetricName, AVG(MetricValue) as AverageValue
                FROM InspectionMetrics 
                WHERE InspectionRequestID = @InspectionRequestID 
                AND IsActive = 1 
                GROUP BY MetricName";

            var results = await _unitOfWork.Connection.QueryAsync<dynamic>(
                sql,
                new { InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);

            return results.ToDictionary(
                result => (string)result.MetricName,
                result => (decimal)result.AverageValue
            );
        }

        public async Task<IEnumerable<InspectionMetric>> GetLatestMetricsByTypeAsync(string metricType, int limit = 10)
        {
            const string sql = @"
                SELECT TOP (@Limit) * FROM InspectionMetrics 
                WHERE MetricType = @MetricType 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                new { MetricType = metricType, Limit = limit },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionMetric> GetLatestMetricByNameAsync(string metricName, int inspectionRequestId)
        {
            const string sql = @"
                SELECT TOP 1 * FROM InspectionMetrics 
                WHERE MetricName = @MetricName 
                AND InspectionRequestID = @InspectionRequestID 
                AND IsActive = 1 
                ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionMetric>(
                sql,
                new { MetricName = metricName, InspectionRequestID = inspectionRequestId },
                _unitOfWork.Transaction);
        }

        public async Task<InspectionMetric> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM InspectionMetrics WHERE MetricID = @MetricID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<InspectionMetric>(
                sql,
                new { MetricID = id },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(InspectionMetric inspectionMetric)
        {
            const string sql = @"
                INSERT INTO InspectionMetrics (
                    InspectionRequestID, MetricType, MetricName, MetricValue, MetricUnit, 
                    TargetValue, Variance, BenchmarkValue, CalculationDate, CalculatedBy, 
                    Notes, IsActive)
                VALUES (
                    @InspectionRequestID, @MetricType, @MetricName, @MetricValue, @MetricUnit, 
                    @TargetValue, @Variance, @BenchmarkValue, @CalculationDate, @CalculatedBy, 
                    @Notes, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set calculation date if not set
            if (inspectionMetric.CalculationDate == default(DateTime))
            {
                inspectionMetric.CalculationDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                inspectionMetric,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(InspectionMetric inspectionMetric)
        {
            const string sql = @"
                UPDATE InspectionMetrics
                SET InspectionRequestID = @InspectionRequestID,
                    MetricType = @MetricType,
                    MetricName = @MetricName,
                    MetricValue = @MetricValue,
                    MetricUnit = @MetricUnit,
                    TargetValue = @TargetValue,
                    Variance = @Variance,
                    BenchmarkValue = @BenchmarkValue,
                    CalculationDate = @CalculationDate,
                    CalculatedBy = @CalculatedBy,
                    Notes = @Notes,
                    IsActive = @IsActive
                WHERE MetricID = @MetricID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                inspectionMetric,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InspectionMetric>> GetAllAsync()
        {
            const string sql = "SELECT * FROM InspectionMetrics ORDER BY CalculationDate DESC";
            return await _unitOfWork.Connection.QueryAsync<InspectionMetric>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        public async Task<PagedResult<InspectionMetric>> GetPagedAsync(int page, int pageSize, string searchTerm = null)
        {
            string whereClause = "1=1";
            object parameters = new { };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += @" AND (
                    MetricType LIKE @SearchTerm OR 
                    MetricName LIKE @SearchTerm OR
                    MetricUnit LIKE @SearchTerm OR
                    Notes LIKE @SearchTerm)";

                parameters = new { SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<InspectionMetric>(
                "InspectionMetrics",
                "CalculationDate DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM InspectionMetrics WHERE MetricID = @MetricID";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { MetricID = id },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}
