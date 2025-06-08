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
    public class WorkflowAssignmentService : IWorkflowAssignmentService
    {
        private readonly IWorkflowAssignmentRepository _workflowAssignmentRepository;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowStepRepository _workflowStepRepository;
        private readonly IWorkflowHistoryRepository _workflowHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowAssignmentService(
            IWorkflowAssignmentRepository workflowAssignmentRepository,
            IWorkflowInstanceRepository workflowInstanceRepository,
            IWorkflowStepRepository workflowStepRepository,
            IWorkflowHistoryRepository workflowHistoryRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowAssignmentRepository = workflowAssignmentRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowStepRepository = workflowStepRepository;
            _workflowHistoryRepository = workflowHistoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowAssignment>> GetWorkflowAssignmentByIdAsync(int assignmentId, int tenantId)
        {
            try
            {
                var assignment = await _workflowAssignmentRepository.GetByIdAsync(assignmentId, tenantId);
                if (assignment == null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("Workflow assignment not found");
                }

                return ServiceResult<WorkflowAssignment>.SuccessResult(assignment);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowAssignment>.FailureResult($"Error retrieving workflow assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowAssignment>> GetActiveAssignmentAsync(int instanceId, int tenantId)
        {
            try
            {
                var assignment = await _workflowAssignmentRepository.GetActiveAssignmentAsync(instanceId, tenantId);
                if (assignment == null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("No active assignment found for this instance");
                }

                return ServiceResult<WorkflowAssignment>.SuccessResult(assignment);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowAssignment>.FailureResult($"Error retrieving active assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByInstanceIdAsync(int instanceId, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByInstanceIdAsync(instanceId, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByAssignedToAsync(string assignedTo, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByAssignedToAsync(assignedTo, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsByAssignedToAsync(string assignedTo, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetActiveByAssignedToAsync(assignedTo, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving active assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByRoleAsync(string assignedRole, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByAssignedRoleAsync(assignedRole, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByStepAsync(int stepId, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByStepIdAsync(stepId, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByAssignedByAsync(string assignedBy, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByAssignedByAsync(assignedBy, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsAsync(int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetActiveAssignmentsAsync(tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving active assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetCompletedAssignmentsAsync(int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetCompletedAssignmentsAsync(tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving completed assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetOverdueAssignmentsAsync(int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetOverdueAssignmentsAsync(tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving overdue assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetAssignmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            try
            {
                if (startDate > endDate)
                {
                    return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult("Start date must be before end date");
                }

                var assignments = await _workflowAssignmentRepository.GetByDateRangeAsync(startDate, endDate, tenantId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetActiveAssignmentCountAsync(string assignedTo, int tenantId)
        {
            try
            {
                var count = await _workflowAssignmentRepository.GetActiveAssignmentCountAsync(assignedTo, tenantId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error counting active assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowAssignment>> CreateAssignmentAsync(int instanceId, int stepId, string assignedTo, string assignedRole, string assignedBy, int tenantId, string notes = null)
        {
            try
            {
                // Verify instance exists and belongs to tenant
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("Workflow instance not found");
                }

                // Verify instance is active
                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("Cannot assign to inactive workflow instance");
                }

                // Verify step exists and belongs to tenant
                var step = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (step == null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("Workflow step not found");
                }

                // Check if there's already an active assignment
                var existingAssignment = await _workflowAssignmentRepository.GetActiveAssignmentAsync(instanceId, tenantId);
                if (existingAssignment != null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("An active assignment already exists for this instance");
                }

                var assignment = new WorkflowAssignment
                {
                    InstanceId = instanceId,
                    StepId = stepId,
                    AssignedTo = assignedTo,
                    AssignedRole = assignedRole,
                    AssignedDate = DateTime.UtcNow,
                    AssignedBy = assignedBy,
                    Notes = notes,
                    IsActive = true
                };

                _unitOfWork.BeginTransaction();

                // Create the assignment
                int assignmentId = await _workflowAssignmentRepository.InsertAsync(assignment);
                assignment.AssignmentId = assignmentId;

                // Update instance assigned to
                instance.AssignedTo = assignedTo;
                await _workflowInstanceRepository.UpdateAsync(instance);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = stepId,
                    ToStepId = stepId,
                    ActionCode = "ASSIGN",
                    Action = $"Assigned to {assignedTo}",
                    Comments = notes,
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = assignedBy,
                    ActionByRole = assignedRole,
                    AssignedTo = assignedTo
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<WorkflowAssignment>.SuccessResult(assignment, "Assignment created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<WorkflowAssignment>.FailureResult($"Error creating assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAssignmentAsync(WorkflowAssignment workflowAssignment, int tenantId)
        {
            try
            {
                // Verify assignment exists and belongs to tenant
                var existingAssignment = await _workflowAssignmentRepository.GetByIdAsync(workflowAssignment.AssignmentId, tenantId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                // Preserve immutable fields
                workflowAssignment.InstanceId = existingAssignment.InstanceId;
                workflowAssignment.StepId = existingAssignment.StepId;
                workflowAssignment.AssignedDate = existingAssignment.AssignedDate;
                workflowAssignment.AssignedBy = existingAssignment.AssignedBy;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowAssignmentRepository.UpdateAsync(workflowAssignment);

                // Update instance assigned to if assignment is active
                if (workflowAssignment.IsActive)
                {
                    var instance = await _workflowInstanceRepository.GetByIdAsync(workflowAssignment.InstanceId, tenantId);
                    if (instance != null)
                    {
                        instance.AssignedTo = workflowAssignment.AssignedTo;
                        await _workflowInstanceRepository.UpdateAsync(instance);
                    }
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, int tenantId, string notes = null)
        {
            try
            {
                var assignment = await _workflowAssignmentRepository.GetByIdAsync(assignmentId, tenantId);
                if (assignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                if (!assignment.IsActive)
                {
                    return ServiceResult<bool>.FailureResult("Assignment is already completed");
                }

                _unitOfWork.BeginTransaction();

                assignment.IsActive = false;
                assignment.CompletedDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(notes))
                {
                    assignment.Notes = string.IsNullOrEmpty(assignment.Notes)
                        ? notes
                        : $"{assignment.Notes} | Completion: {notes}";
                }

                await _workflowAssignmentRepository.UpdateAsync(assignment);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = assignment.InstanceId,
                    FromStepId = assignment.StepId,
                    ToStepId = assignment.StepId,
                    ActionCode = "COMPLETE_ASSIGNMENT",
                    Action = "Assignment completed",
                    Comments = notes,
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = assignment.AssignedTo,
                    ActionByRole = assignment.AssignedRole
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Assignment completed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error completing assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReassignAsync(int assignmentId, string newAssignedTo, string reassignedBy, int tenantId, string notes = null)
        {
            try
            {
                var assignment = await _workflowAssignmentRepository.GetByIdAsync(assignmentId, tenantId);
                if (assignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                if (!assignment.IsActive)
                {
                    return ServiceResult<bool>.FailureResult("Cannot reassign completed assignment");
                }

                var previousAssignedTo = assignment.AssignedTo;

                _unitOfWork.BeginTransaction();

                // Deactivate current assignment
                assignment.IsActive = false;
                assignment.CompletedDate = DateTime.UtcNow;
                assignment.Notes = string.IsNullOrEmpty(assignment.Notes)
                    ? $"Reassigned to {newAssignedTo}"
                    : $"{assignment.Notes} | Reassigned to {newAssignedTo}";

                await _workflowAssignmentRepository.UpdateAsync(assignment);

                // Create new assignment
                var newAssignment = new WorkflowAssignment
                {
                    InstanceId = assignment.InstanceId,
                    StepId = assignment.StepId,
                    AssignedTo = newAssignedTo,
                    AssignedRole = assignment.AssignedRole,
                    AssignedDate = DateTime.UtcNow,
                    AssignedBy = reassignedBy,
                    Notes = $"Reassigned from {previousAssignedTo}" + (string.IsNullOrEmpty(notes) ? "" : $" | {notes}"),
                    IsActive = true
                };

                await _workflowAssignmentRepository.InsertAsync(newAssignment);

                // Update instance
                var instance = await _workflowInstanceRepository.GetByIdAsync(assignment.InstanceId, tenantId);
                if (instance != null)
                {
                    instance.AssignedTo = newAssignedTo;
                    await _workflowInstanceRepository.UpdateAsync(instance);
                }

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = assignment.InstanceId,
                    FromStepId = assignment.StepId,
                    ToStepId = assignment.StepId,
                    ActionCode = "REASSIGN",
                    Action = $"Reassigned from {previousAssignedTo} to {newAssignedTo}",
                    Comments = notes,
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = reassignedBy,
                    AssignedTo = newAssignedTo
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Assignment reassigned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reassigning: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> BulkAssignAsync(List<int> instanceIds, string assignedTo, string assignedRole, string assignedBy, int tenantId)
        {
            try
            {
                if (instanceIds == null || !instanceIds.Any())
                {
                    return ServiceResult<bool>.FailureResult("No instances provided for bulk assignment");
                }

                _unitOfWork.BeginTransaction();

                int successCount = 0;
                var failedInstances = new List<int>();

                foreach (var instanceId in instanceIds)
                {
                    try
                    {
                        // Get instance
                        var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                        if (instance == null || instance.Status != WorkflowStatusConstants.Active)
                        {
                            failedInstances.Add(instanceId);
                            continue;
                        }

                        // Check for existing active assignment
                        var existingAssignment = await _workflowAssignmentRepository.GetActiveAssignmentAsync(instanceId, tenantId);
                        if (existingAssignment != null)
                        {
                            // Deactivate existing assignment
                            existingAssignment.IsActive = false;
                            existingAssignment.CompletedDate = DateTime.UtcNow;
                            existingAssignment.Notes = $"Bulk reassigned to {assignedTo}";
                            await _workflowAssignmentRepository.UpdateAsync(existingAssignment);
                        }

                        // Create new assignment
                        var assignment = new WorkflowAssignment
                        {
                            InstanceId = instanceId,
                            StepId = instance.CurrentStepId,
                            AssignedTo = assignedTo,
                            AssignedRole = assignedRole,
                            AssignedDate = DateTime.UtcNow,
                            AssignedBy = assignedBy,
                            Notes = "Bulk assignment",
                            IsActive = true
                        };

                        await _workflowAssignmentRepository.InsertAsync(assignment);

                        // Update instance
                        instance.AssignedTo = assignedTo;
                        await _workflowInstanceRepository.UpdateAsync(instance);

                        successCount++;
                    }
                    catch
                    {
                        failedInstances.Add(instanceId);
                    }
                }

                _unitOfWork.Commit();

                var message = $"Successfully assigned {successCount} out of {instanceIds.Count} instances";
                if (failedInstances.Any())
                {
                    message += $". Failed instances: {string.Join(", ", failedInstances)}";
                }

                return ServiceResult<bool>.SuccessResult(successCount > 0, message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error performing bulk assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateOldAssignmentsAsync(int instanceId, int tenantId)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetByInstanceIdAsync(instanceId, tenantId);
                var activeAssignments = assignments.Where(a => a.IsActive).ToList();

                if (!activeAssignments.Any())
                {
                    return ServiceResult<bool>.SuccessResult(true, "No active assignments to deactivate");
                }

                _unitOfWork.BeginTransaction();

                foreach (var assignment in activeAssignments)
                {
                    assignment.IsActive = false;
                    assignment.CompletedDate = DateTime.UtcNow;
                    assignment.Notes = string.IsNullOrEmpty(assignment.Notes)
                        ? "Deactivated due to workflow transition"
                        : $"{assignment.Notes} | Deactivated due to workflow transition";

                    await _workflowAssignmentRepository.UpdateAsync(assignment);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, $"Deactivated {activeAssignments.Count} assignments");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int assignmentId, int tenantId)
        {
            try
            {
                var assignment = await _workflowAssignmentRepository.GetByIdAsync(assignmentId, tenantId);
                if (assignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowAssignmentRepository.DeleteAsync(assignmentId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByInstanceIdAsync(int instanceId, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowAssignmentRepository.DeleteByInstanceIdAsync(instanceId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "All assignments deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting assignments: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetWorkflowAssignmentsAsync(int tenantId)
        {
            return await _workflowAssignmentRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<WorkflowAssignment>>> GetPagedWorkflowAssignmentsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var assignments = await _workflowAssignmentRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowAssignment>>.FailureResult($"Error retrieving assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<string, object>>> GetAssignmentStatisticsAsync(string assignedTo, int tenantId)
        {
            try
            {
                var statistics = new Dictionary<string, object>();

                // Get all assignments for the user
                var allAssignments = await _workflowAssignmentRepository.GetByAssignedToAsync(assignedTo, tenantId);
                var assignmentsList = allAssignments.ToList();

                statistics["TotalAssignments"] = assignmentsList.Count;
                statistics["ActiveAssignments"] = assignmentsList.Count(a => a.IsActive);
                statistics["CompletedAssignments"] = assignmentsList.Count(a => !a.IsActive && a.CompletedDate.HasValue);

                // Calculate average completion time
                var completedWithTime = assignmentsList
                    .Where(a => a.CompletedDate.HasValue)
                    .Select(a => (a.CompletedDate.Value - a.AssignedDate).TotalHours)
                    .ToList();

                if (completedWithTime.Any())
                {
                    statistics["AverageCompletionHours"] = Math.Round(completedWithTime.Average(), 2);
                    statistics["MinCompletionHours"] = Math.Round(completedWithTime.Min(), 2);
                    statistics["MaxCompletionHours"] = Math.Round(completedWithTime.Max(), 2);
                }

                // Assignment by role distribution
                var roleDistribution = assignmentsList
                    .GroupBy(a => a.AssignedRole)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .OrderByDescending(r => r.Count)
                    .ToList();

                statistics["RoleDistribution"] = roleDistribution;

                // Recent activity (last 30 days)
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var recentAssignments = assignmentsList.Where(a => a.AssignedDate >= thirtyDaysAgo).ToList();
                statistics["RecentAssignments"] = recentAssignments.Count;
                statistics["RecentCompletions"] = recentAssignments.Count(a => a.CompletedDate.HasValue);

                // Overdue assignments
                var overdueAssignments = await _workflowAssignmentRepository.GetOverdueAssignmentsAsync(tenantId);
                var userOverdueCount = overdueAssignments.Count(a => a.AssignedTo == assignedTo);
                statistics["OverdueAssignments"] = userOverdueCount;

                return ServiceResult<Dictionary<string, object>>.SuccessResult(statistics, "Assignment statistics calculated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.FailureResult($"Error calculating assignment statistics: {ex.Message}");
            }
        }
    }
}
