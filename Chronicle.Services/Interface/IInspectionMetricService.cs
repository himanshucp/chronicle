using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IInspectionMetricService
    {
        Task<ServiceResult<InspectionMetric>> GetInspectionMetricByIdAsync(int metricId);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByInspectionRequestIdAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByMetricTypeAsync(string metricType);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByMetricNameAsync(string metricName);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByCalculatedByAsync(int calculatedBy);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByCalculationDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsAboveTargetAsync();
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsBelowTargetAsync();
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsWithinVarianceAsync(decimal varianceThreshold);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsAboveBenchmarkAsync();
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsBelowBenchmarkAsync();
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetTrendDataAsync(string metricName, DateTime fromDate, DateTime toDate);
        Task<ServiceResult<Dictionary<string, decimal>>> GetMetricSummaryAsync(int inspectionRequestId);
        Task<ServiceResult<IEnumerable<InspectionMetric>>> GetLatestMetricsByTypeAsync(string metricType, int limit = 10);
        Task<ServiceResult<InspectionMetric>> GetLatestMetricByNameAsync(string metricName, int inspectionRequestId);
        Task<ServiceResult<decimal>> GetAverageMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ServiceResult<decimal>> GetMaxMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ServiceResult<decimal>> GetMinMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<InspectionMetric>> GetInspectionMetricsAsync();
        Task<ServiceResult<PagedResult<InspectionMetric>>> GetPagedInspectionMetricsAsync(int page, int pageSize, string searchTerm = null);
        Task<ServiceResult<int>> CreateInspectionMetricAsync(InspectionMetric inspectionMetric);
        Task<ServiceResult<bool>> UpdateAsync(InspectionMetric inspectionMetric);
        Task<ServiceResult<bool>> CalculateVarianceAsync(int metricId);
        Task<ServiceResult<bool>> UpdateBenchmarkAsync(int metricId, decimal benchmarkValue);
        Task<ServiceResult<bool>> UpdateTargetAsync(int metricId, decimal targetValue);
        Task<ServiceResult<bool>> RecalculateMetricAsync(int metricId, int calculatedBy);
        Task<ServiceResult<bool>> DeleteAsync(int metricId,int tenantId);
    }
}
