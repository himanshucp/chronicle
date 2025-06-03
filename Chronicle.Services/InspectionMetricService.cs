using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class InspectionMetricService : IInspectionMetricService
    {
        private readonly IInspectionMetricRepository _inspectionMetricRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionMetricService(
            IInspectionMetricRepository inspectionMetricRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionMetricRepository = inspectionMetricRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionMetric>> GetInspectionMetricByIdAsync(int metricId)
        {
            try
            {
                var inspectionMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (inspectionMetric == null)
                {
                    return ServiceResult<InspectionMetric>.FailureResult("Inspection metric not found");
                }

                return ServiceResult<InspectionMetric>.SuccessResult(inspectionMetric);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionMetric>.FailureResult($"Error retrieving inspection metric: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByMetricTypeAsync(string metricType)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetByMetricTypeAsync(metricType);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByMetricNameAsync(string metricName)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetByMetricNameAsync(metricName);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByCalculatedByAsync(int calculatedBy)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetByCalculatedByAsync(calculatedBy);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetByCalculationDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetByCalculationDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsAboveTargetAsync()
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetMetricsAboveTargetAsync();
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving metrics above target: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsBelowTargetAsync()
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetMetricsBelowTargetAsync();
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving metrics below target: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsWithinVarianceAsync(decimal varianceThreshold)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetMetricsWithinVarianceAsync(varianceThreshold);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving metrics within variance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsAboveBenchmarkAsync()
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetMetricsAboveBenchmarkAsync();
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving metrics above benchmark: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetMetricsBelowBenchmarkAsync()
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetMetricsBelowBenchmarkAsync();
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving metrics below benchmark: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetTrendDataAsync(string metricName, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetTrendDataAsync(metricName, fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving trend data: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<string, decimal>>> GetMetricSummaryAsync(int inspectionRequestId)
        {
            try
            {
                var summary = await _inspectionMetricRepository.GetMetricSummaryAsync(inspectionRequestId);
                return ServiceResult<Dictionary<string, decimal>>.SuccessResult(summary);
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, decimal>>.FailureResult($"Error retrieving metric summary: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionMetric>>> GetLatestMetricsByTypeAsync(string metricType, int limit = 10)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetLatestMetricsByTypeAsync(metricType, limit);
                return ServiceResult<IEnumerable<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionMetric>>.FailureResult($"Error retrieving latest metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<InspectionMetric>> GetLatestMetricByNameAsync(string metricName, int inspectionRequestId)
        {
            try
            {
                var inspectionMetric = await _inspectionMetricRepository.GetLatestMetricByNameAsync(metricName, inspectionRequestId);
                if (inspectionMetric == null)
                {
                    return ServiceResult<InspectionMetric>.FailureResult("Latest metric not found");
                }

                return ServiceResult<InspectionMetric>.SuccessResult(inspectionMetric);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionMetric>.FailureResult($"Error retrieving latest metric: {ex.Message}");
            }
        }

        public async Task<ServiceResult<decimal>> GetAverageMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var average = await _inspectionMetricRepository.GetAverageMetricValueAsync(metricName, fromDate, toDate);
                return ServiceResult<decimal>.SuccessResult(average);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.FailureResult($"Error calculating average metric value: {ex.Message}");
            }
        }

        public async Task<ServiceResult<decimal>> GetMaxMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var maximum = await _inspectionMetricRepository.GetMaxMetricValueAsync(metricName, fromDate, toDate);
                return ServiceResult<decimal>.SuccessResult(maximum);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.FailureResult($"Error calculating maximum metric value: {ex.Message}");
            }
        }

        public async Task<ServiceResult<decimal>> GetMinMetricValueAsync(string metricName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var minimum = await _inspectionMetricRepository.GetMinMetricValueAsync(metricName, fromDate, toDate);
                return ServiceResult<decimal>.SuccessResult(minimum);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.FailureResult($"Error calculating minimum metric value: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionMetricAsync(InspectionMetric inspectionMetric)
        {
            try
            {
                // Set default values
                inspectionMetric.CalculationDate = DateTime.UtcNow;
                inspectionMetric.IsActive = true;

                // Calculate variance if target value is provided
                if (inspectionMetric.TargetValue.HasValue)
                {
                    inspectionMetric.Variance = inspectionMetric.MetricValue - inspectionMetric.TargetValue.Value;
                }

                _unitOfWork.BeginTransaction();

                int metricId = await _inspectionMetricRepository.InsertAsync(inspectionMetric);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(metricId, "Inspection metric created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection metric: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionMetric inspectionMetric)
        {
            try
            {
                // Check if inspection metric exists
                var existingInspectionMetric = await _inspectionMetricRepository.GetByIdAsync(inspectionMetric.MetricID);
                if (existingInspectionMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                // Recalculate variance if target value is provided
                if (inspectionMetric.TargetValue.HasValue)
                {
                    inspectionMetric.Variance = inspectionMetric.MetricValue - inspectionMetric.TargetValue.Value;
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.UpdateAsync(inspectionMetric);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection metric updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection metric: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CalculateVarianceAsync(int metricId)
        {
            try
            {
                var existingMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (existingMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                if (!existingMetric.TargetValue.HasValue)
                {
                    return ServiceResult<bool>.FailureResult("Cannot calculate variance without target value");
                }

                existingMetric.Variance = existingMetric.MetricValue - existingMetric.TargetValue.Value;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.UpdateAsync(existingMetric);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Variance calculated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error calculating variance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateBenchmarkAsync(int metricId, decimal benchmarkValue)
        {
            try
            {
                var existingMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (existingMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                existingMetric.BenchmarkValue = benchmarkValue;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.UpdateAsync(existingMetric);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Benchmark value updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating benchmark value: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateTargetAsync(int metricId, decimal targetValue)
        {
            try
            {
                var existingMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (existingMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                existingMetric.TargetValue = targetValue;
                // Recalculate variance with new target
                existingMetric.Variance = existingMetric.MetricValue - targetValue;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.UpdateAsync(existingMetric);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Target value updated and variance recalculated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating target value: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RecalculateMetricAsync(int metricId, int calculatedBy)
        {
            try
            {
                var existingMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (existingMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                existingMetric.CalculationDate = DateTime.UtcNow;
                existingMetric.CalculatedBy = calculatedBy;

                // Recalculate variance if target value exists
                if (existingMetric.TargetValue.HasValue)
                {
                    existingMetric.Variance = existingMetric.MetricValue - existingMetric.TargetValue.Value;
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.UpdateAsync(existingMetric);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Metric recalculated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error recalculating metric: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int metricId,int tenantId)
        {
            try
            {
                // Check if inspection metric exists
                var existingInspectionMetric = await _inspectionMetricRepository.GetByIdAsync(metricId);
                if (existingInspectionMetric == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection metric not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionMetricRepository.DeleteAsync(metricId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection metric deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection metric: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionMetric>> GetInspectionMetricsAsync()
        {
            return await _inspectionMetricRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectionMetric>>> GetPagedInspectionMetricsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectionMetrics = await _inspectionMetricRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectionMetric>>.SuccessResult(inspectionMetrics);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionMetric>>.FailureResult($"Error retrieving inspection metrics: {ex.Message}");
            }
        }
    }
}
