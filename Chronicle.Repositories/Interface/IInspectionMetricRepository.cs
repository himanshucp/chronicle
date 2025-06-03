using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IInspectionMetricRepository : IRepository<InspectionMetric, int>
    {
        Task<IEnumerable<InspectionMetric>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionMetric>> GetByMetricTypeAsync(string metricType);
        Task<IEnumerable<InspectionMetric>> GetByMetricNameAsync(string metricName);
        Task<IEnumerable<InspectionMetric>> GetByCalculatedByAsync(int calculatedBy);
        Task<IEnumerable<InspectionMetric>> GetByCalculationDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<InspectionMetric>> GetMetricsAboveTargetAsync();
        Task<IEnumerable<InspectionMetric>> GetMetricsBelowTargetAsync();
        Task<IEnumerable<InspectionMetric>> GetMetricsWithinVarianceAsync(decimal varianceThreshold);
        Task<IEnumerable<InspectionMetric>> GetMetricsAboveBenchmarkAsync();
        Task<IEnumerable<InspectionMetric>> GetMetricsBelowBenchmarkAsync();
        Task<decimal> GetAverageMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetMaxMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetMinMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<InspectionMetric>> GetTrendDataAsync(string metricName, DateTime fromDate, DateTime toDate);
        Task<Dictionary<string, decimal>> GetMetricSummaryAsync(int inspectionRequestId);
        Task<IEnumerable<InspectionMetric>> GetLatestMetricsByTypeAsync(string metricType, int limit = 10);
        Task<InspectionMetric> GetLatestMetricByNameAsync(string metricName, int inspectionRequestId);
        Task<InspectionMetric> GetByIdAsync(int id);
        Task<IEnumerable<InspectionMetric>> GetAllAsync();
        Task<PagedResult<InspectionMetric>> GetPagedAsync(int page, int pageSize, string searchTerm = null);
    }
}
