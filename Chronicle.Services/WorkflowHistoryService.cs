using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using Chronicle.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class WorkflowHistoryService : IWorkflowHistoryService
    {
        private readonly IWorkflowHistoryRepository _workflowHistoryRepository;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowStepRepository _workflowStepRepository;
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowHistoryService(
            IWorkflowHistoryRepository workflowHistoryRepository,
            IWorkflowInstanceRepository workflowInstanceRepository,
            IWorkflowStepRepository workflowStepRepository,
            IWorkflowTransitionRepository workflowTransitionRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowHistoryRepository = workflowHistoryRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowStepRepository = workflowStepRepository;
            _workflowTransitionRepository = workflowTransitionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowHistory>> GetWorkflowHistoryByIdAsync(int historyId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByIdAsync(historyId, tenantId);
                if (history == null)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult("Workflow history entry not found");
                }

                return ServiceResult<WorkflowHistory>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowHistory>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowHistory>> GetHistoryWithStepsAsync(int historyId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByIdWithStepsAsync(historyId, tenantId);
                if (history == null)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult("Workflow history entry not found");
                }

                return ServiceResult<WorkflowHistory>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowHistory>.FailureResult($"Error retrieving workflow history with steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByInstanceIdAsync(int instanceId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByInstanceIdAsync(instanceId, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByActionByAsync(string actionBy, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByActionByAsync(actionBy, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByActionCodeAsync(string actionCode, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByActionCodeAsync(actionCode, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByFromStepAsync(int fromStepId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByFromStepAsync(fromStepId, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByToStepAsync(int toStepId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByToStepAsync(toStepId, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            try
            {
                if (startDate > endDate)
                {
                    return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult("Start date must be before end date");
                }

                var history = await _workflowHistoryRepository.GetByDateRangeAsync(startDate, endDate, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByWorkflowIdAsync(int workflowId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetHistoryByAssignedToAsync(string assignedTo, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByAssignedToAsync(assignedTo, tenantId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowHistory>> GetLastTransitionAsync(int instanceId, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetLastTransitionAsync(instanceId, tenantId);
                if (history == null)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult("No transitions found for this instance");
                }

                return ServiceResult<WorkflowHistory>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowHistory>.FailureResult($"Error retrieving last transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<double>> GetAverageStepDurationAsync(int fromStepId, int toStepId, int tenantId)
        {
            try
            {
                var averageDuration = await _workflowHistoryRepository.GetAverageStepDurationAsync(fromStepId, toStepId, tenantId);
                return ServiceResult<double>.SuccessResult(averageDuration);
            }
            catch (Exception ex)
            {
                return ServiceResult<double>.FailureResult($"Error calculating average duration: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetTransitionCountAsync(int fromStepId, int toStepId, int tenantId)
        {
            try
            {
                var count = await _workflowHistoryRepository.GetTransitionCountAsync(fromStepId, toStepId, tenantId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error counting transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateHistoryEntryAsync(WorkflowHistory workflowHistory, int tenantId)
        {
            try
            {
                // Verify instance exists and belongs to tenant
                var instance = await _workflowInstanceRepository.GetByIdAsync(workflowHistory.InstanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow instance not found");
                }

                // Verify steps exist if provided
                if (workflowHistory.FromStepId > 0)
                {
                    var fromStep = await _workflowStepRepository.GetByIdAsync(workflowHistory.FromStepId, tenantId);
                    if (fromStep == null)
                    {
                        return ServiceResult<int>.FailureResult("From step not found");
                    }
                }

                if (workflowHistory.ToStepId > 0)
                {
                    var toStep = await _workflowStepRepository.GetByIdAsync(workflowHistory.ToStepId, tenantId);
                    if (toStep == null)
                    {
                        return ServiceResult<int>.FailureResult("To step not found");
                    }
                }

                // Validate JSON data if provided
                if (!string.IsNullOrEmpty(workflowHistory.AdditionalData))
                {
                    try
                    {
                        JsonDocument.Parse(workflowHistory.AdditionalData);
                    }
                    catch
                    {
                        return ServiceResult<int>.FailureResult("Invalid JSON format for additional data");
                    }
                }

                // Calculate duration if from step is provided
                if (workflowHistory.FromStepId > 0 && workflowHistory.DurationMinutes == null)
                {
                    var lastTransition = await _workflowHistoryRepository.GetLastTransitionAsync(workflowHistory.InstanceId, tenantId);
                    if (lastTransition != null)
                    {
                        var duration = (workflowHistory.TransitionDate - lastTransition.TransitionDate).TotalMinutes;
                        workflowHistory.DurationMinutes = Math.Round(duration, 2);
                    }
                }

                _unitOfWork.BeginTransaction();

                int historyId = await _workflowHistoryRepository.InsertAsync(workflowHistory);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(historyId, "History entry created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating history entry: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowHistory>> CreateTransitionHistoryAsync(int instanceId, int? fromStepId, int? toStepId, int? transitionId, string action, string actionBy, int tenantId, string comments = null, string actionByRole = null)
        {
            try
            {
                // Verify instance exists
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult("Workflow instance not found");
                }

                // Get transition details if transition ID provided
                string actionCode = action;
                if (transitionId.HasValue)
                {
                    var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId.Value, tenantId);
                    if (transition != null)
                    {
                        actionCode = transition.ActionCode;
                    }
                }

                var workflowHistory = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = fromStepId ?? 0,
                    ToStepId = toStepId ?? 0,
                    ActionCode = actionCode,
                    Action = action,
                    Comments = comments,
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = actionBy,
                    ActionByRole = actionByRole,
                    AssignedTo = instance.AssignedTo,
                    DueDate = instance.DueDate
                };

                var result = await CreateHistoryEntryAsync(workflowHistory, tenantId);
                if (!result.Success)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult(result.Message);
                }

                workflowHistory.HistoryId = result.Data;
                return ServiceResult<WorkflowHistory>.SuccessResult(workflowHistory, "Transition history created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowHistory>.FailureResult($"Error creating transition history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateHistoryAsync(WorkflowHistory workflowHistory, int tenantId)
        {
            try
            {
                // Verify history entry exists and belongs to tenant
                var existingHistory = await _workflowHistoryRepository.GetByIdAsync(workflowHistory.HistoryId, tenantId);
                if (existingHistory == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow history entry not found");
                }

                // Preserve immutable fields
                workflowHistory.InstanceId = existingHistory.InstanceId;
                workflowHistory.FromStepId = existingHistory.FromStepId;
                workflowHistory.ToStepId = existingHistory.ToStepId;
                workflowHistory.ActionCode = existingHistory.ActionCode;
                workflowHistory.Action = existingHistory.Action;
                workflowHistory.TransitionDate = existingHistory.TransitionDate;
                workflowHistory.ActionBy = existingHistory.ActionBy;

                // Validate JSON data if provided
                if (!string.IsNullOrEmpty(workflowHistory.AdditionalData))
                {
                    try
                    {
                        JsonDocument.Parse(workflowHistory.AdditionalData);
                    }
                    catch
                    {
                        return ServiceResult<bool>.FailureResult("Invalid JSON format for additional data");
                    }
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowHistoryRepository.UpdateAsync(workflowHistory);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "History entry updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating history entry: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateDurationAsync(int historyId, double durationMinutes, int tenantId)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetByIdAsync(historyId, tenantId);
                if (history == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow history entry not found");
                }

                if (durationMinutes < 0)
                {
                    return ServiceResult<bool>.FailureResult("Duration cannot be negative");
                }

                _unitOfWork.BeginTransaction();

                history.DurationMinutes = durationMinutes;

                bool result = await _workflowHistoryRepository.UpdateAsync(history);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Duration updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating duration: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int historyId, int tenantId)
        {
            try
            {
                // Check if history entry exists and belongs to tenant
                var existingHistory = await _workflowHistoryRepository.GetByIdAsync(historyId, tenantId);
                if (existingHistory == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow history entry not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowHistoryRepository.DeleteAsync(historyId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "History entry deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting history entry: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByInstanceIdAsync(int instanceId, int tenantId)
        {
            try
            {
                // Verify instance exists and belongs to tenant
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowHistoryRepository.DeleteByInstanceIdAsync(instanceId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "All history entries deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting history entries: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkflowHistory>> GetWorkflowHistoryAsync(int tenantId)
        {
            return await _workflowHistoryRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<WorkflowHistory>>> GetPagedWorkflowHistoryAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var history = await _workflowHistoryRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<string, object>>> GetWorkflowMetricsAsync(int workflowId, int tenantId)
        {
            try
            {
                var metrics = new Dictionary<string, object>();

                // Get all history for the workflow
                var history = await _workflowHistoryRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                var historyList = history.ToList();

                if (!historyList.Any())
                {
                    return ServiceResult<Dictionary<string, object>>.SuccessResult(metrics, "No history data available");
                }

                // Total transitions
                metrics["TotalTransitions"] = historyList.Count;

                // Average duration per step
                var stepDurations = historyList
                    .Where(h => h.DurationMinutes.HasValue && h.FromStepId > 0 && h.ToStepId > 0)
                    .GroupBy(h => new { h.FromStepId, h.ToStepId })
                    .Select(g => new
                    {
                        FromStep = g.Key.FromStepId,
                        ToStep = g.Key.ToStepId,
                        AverageDuration = g.Average(h => h.DurationMinutes.Value),
                        Count = g.Count()
                    })
                    .ToList();

                metrics["StepDurations"] = stepDurations;

                // Most active users
                var activeUsers = historyList
                    .GroupBy(h => h.ActionBy)
                    .Select(g => new { User = g.Key, ActionCount = g.Count() })
                    .OrderByDescending(u => u.ActionCount)
                    .Take(10)
                    .ToList();

                metrics["MostActiveUsers"] = activeUsers;

                // Action distribution
                var actionDistribution = historyList
                    .GroupBy(h => h.ActionCode)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .OrderByDescending(a => a.Count)
                    .ToList();

                metrics["ActionDistribution"] = actionDistribution;

                // Average time to completion
                var completedInstances = historyList
                    .Where(h => h.Action == "COMPLETE" || h.ActionCode == "COMPLETE")
                    .Select(h => h.InstanceId)
                    .Distinct()
                    .ToList();

                if (completedInstances.Any())
                {
                    var totalCompletionTime = 0.0;
                    var validCompletions = 0;

                    foreach (var instanceId in completedInstances)
                    {
                        var instanceHistory = historyList.Where(h => h.InstanceId == instanceId).OrderBy(h => h.TransitionDate).ToList();
                        if (instanceHistory.Count >= 2)
                        {
                            var duration = (instanceHistory.Last().TransitionDate - instanceHistory.First().TransitionDate).TotalMinutes;
                            totalCompletionTime += duration;
                            validCompletions++;
                        }
                    }

                    if (validCompletions > 0)
                    {
                        metrics["AverageCompletionTimeMinutes"] = Math.Round(totalCompletionTime / validCompletions, 2);
                    }
                }

                return ServiceResult<Dictionary<string, object>>.SuccessResult(metrics, "Workflow metrics calculated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.FailureResult($"Error calculating workflow metrics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetUserActivityAsync(string userId, DateTime? startDate, DateTime? endDate, int tenantId)
        {
            try
            {
                // Get all history for the user
                var history = await _workflowHistoryRepository.GetByActionByAsync(userId, tenantId);
                var historyList = history.ToList();

                // Filter by date range if provided
                if (startDate.HasValue)
                {
                    historyList = historyList.Where(h => h.TransitionDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    historyList = historyList.Where(h => h.TransitionDate <= endDate.Value).ToList();
                }

                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(historyList);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving user activity: {ex.Message}");
            }
        }
    }
}
