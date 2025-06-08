using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class WorkflowInstanceService : IWorkflowInstanceService
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowStepRepository _workflowStepRepository;
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        private readonly IWorkflowHistoryRepository _workflowHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowInstanceService(
            IWorkflowInstanceRepository workflowInstanceRepository,
            IWorkflowRepository workflowRepository,
            IWorkflowStepRepository workflowStepRepository,
            IWorkflowTransitionRepository workflowTransitionRepository,
            IWorkflowHistoryRepository workflowHistoryRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowRepository = workflowRepository;
            _workflowStepRepository = workflowStepRepository;
            _workflowTransitionRepository = workflowTransitionRepository;
            _workflowHistoryRepository = workflowHistoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowInstance>> GetWorkflowInstanceByIdAsync(int instanceId, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("Workflow instance not found");
                }

                return ServiceResult<WorkflowInstance>.SuccessResult(instance);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowInstance>.FailureResult($"Error retrieving workflow instance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowInstance>> GetInstanceWithHistoryAsync(int instanceId, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdWithHistoryAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("Workflow instance not found");
                }

                return ServiceResult<WorkflowInstance>.SuccessResult(instance);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowInstance>.FailureResult($"Error retrieving workflow instance with history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowInstance>> GetByEntityAsync(int entityId, string entityType, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByEntityAsync(entityId, entityType, tenantId);
                if (instance == null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("No active workflow instance found for this entity");
                }

                return ServiceResult<WorkflowInstance>.SuccessResult(instance);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowInstance>.FailureResult($"Error retrieving workflow instance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByWorkflowIdAsync(int workflowId, int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByCurrentStepAsync(int stepId, int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetByCurrentStepAsync(stepId, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByStatusAsync(string status, int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetByStatusAsync(status, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetActiveInstancesAsync(int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetActiveInstancesAsync(tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving active instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByAssignedToAsync(string assignedTo, int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetByAssignedToAsync(assignedTo, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving assigned instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetOverdueInstancesAsync(int tenantId)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetOverdueInstancesAsync(tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving overdue instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowInstance>> StartWorkflowAsync(int workflowId, int entityId, string entityType, string createdBy, int tenantId, string data = null, string variables = null)
        {
            try
            {
                // Verify workflow exists and is active
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("Workflow not found");
                }

                if (!workflow.IsActive)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("Workflow is not active");
                }

                // Check if an active instance already exists for this entity
                var existingInstance = await _workflowInstanceRepository.GetByEntityAsync(entityId, entityType, tenantId);
                if (existingInstance != null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("An active workflow instance already exists for this entity");
                }

                // Get the initial step
                var initialStep = await _workflowStepRepository.GetInitialStepAsync(workflowId, tenantId);
                if (initialStep == null)
                {
                    return ServiceResult<WorkflowInstance>.FailureResult("Workflow has no initial step defined");
                }

                // Validate JSON data if provided
                if (!string.IsNullOrEmpty(data))
                {
                    try
                    {
                        JsonDocument.Parse(data);
                    }
                    catch
                    {
                        return ServiceResult<WorkflowInstance>.FailureResult("Invalid JSON format for data");
                    }
                }

                if (!string.IsNullOrEmpty(variables))
                {
                    try
                    {
                        JsonDocument.Parse(variables);
                    }
                    catch
                    {
                        return ServiceResult<WorkflowInstance>.FailureResult("Invalid JSON format for variables");
                    }
                }

                // Create new workflow instance
                var workflowInstance = new WorkflowInstance
                {
                    WorkflowId = workflowId,
                    EntityId = entityId,
                    EntityType = entityType,
                    CurrentStepId = initialStep.StepId,
                    Status = WorkflowStatusConstants.Active,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    Data = data,
                    Variables = variables,
                    Priority = WorkflowPriorityConstants.Normal,
                    LastTransitionDate = DateTime.UtcNow
                };

                _unitOfWork.BeginTransaction();

                // Insert the instance
                int instanceId = await _workflowInstanceRepository.InsertAsync(workflowInstance);
                workflowInstance.InstanceId = instanceId;

                // Create initial history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = 0, // Use 0 instead of null for non-nullable int
                    ToStepId = initialStep.StepId,
                    Action = "START",
                    ActionCode = "START",
                    ActionBy = createdBy,
                    TransitionDate = DateTime.UtcNow,
                    Comments = "Workflow started"
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<WorkflowInstance>.SuccessResult(workflowInstance, "Workflow started successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<WorkflowInstance>.FailureResult($"Error starting workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> TransitionWorkflowAsync(int instanceId, int transitionId, string performedBy, int tenantId, string comments = null)
        {
            try
            {
                // Get the instance
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance is not active");
                }

                // Get the transition
                var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<bool>.FailureResult("Transition not found");
                }

                // Verify transition is from current step
                if (transition.FromStepId != instance.CurrentStepId)
                {
                    return ServiceResult<bool>.FailureResult("Invalid transition for current step");
                }

                // Check if comments are required
                if (transition.RequiresComments && string.IsNullOrWhiteSpace(comments))
                {
                    return ServiceResult<bool>.FailureResult("Comments are required for this transition");
                }

                // Get the target step
                var toStep = await _workflowStepRepository.GetByIdAsync(transition.ToStepId, tenantId);
                if (toStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Target step not found");
                }

                _unitOfWork.BeginTransaction();

                // Update instance
                instance.CurrentStepId = transition.ToStepId;
                instance.LastTransitionDate = DateTime.UtcNow;

                // Check if target step is final
                if (toStep.IsFinal)
                {
                    instance.Status = WorkflowStatusConstants.Completed;
                    instance.CompletedDate = DateTime.UtcNow;
                    instance.CompletedBy = performedBy;
                }

                await _workflowInstanceRepository.UpdateAsync(instance);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = transition.FromStepId,
                    ToStepId = transition.ToStepId,
                    Action = transition.ActionCode,
                    ActionCode = transition.ActionCode,
                    ActionBy = performedBy,
                    TransitionDate = DateTime.UtcNow,
                    Comments = comments,
                    AdditionalData = instance.Data
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow transitioned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error transitioning workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateInstanceAsync(WorkflowInstance workflowInstance, int tenantId)
        {
            try
            {
                // Verify instance exists and belongs to tenant
                var existingInstance = await _workflowInstanceRepository.GetByIdAsync(workflowInstance.InstanceId, tenantId);
                if (existingInstance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                // Preserve immutable fields
                workflowInstance.WorkflowId = existingInstance.WorkflowId;
                workflowInstance.EntityId = existingInstance.EntityId;
                workflowInstance.EntityType = existingInstance.EntityType;
                workflowInstance.CreatedDate = existingInstance.CreatedDate;
                workflowInstance.CreatedBy = existingInstance.CreatedBy;

                // Validate JSON fields if provided
                if (!string.IsNullOrEmpty(workflowInstance.Data))
                {
                    try
                    {
                        JsonDocument.Parse(workflowInstance.Data);
                    }
                    catch
                    {
                        return ServiceResult<bool>.FailureResult("Invalid JSON format for data");
                    }
                }

                if (!string.IsNullOrEmpty(workflowInstance.Variables))
                {
                    try
                    {
                        JsonDocument.Parse(workflowInstance.Variables);
                    }
                    catch
                    {
                        return ServiceResult<bool>.FailureResult("Invalid JSON format for variables");
                    }
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowInstanceRepository.UpdateAsync(workflowInstance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow instance updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow instance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CompleteWorkflowAsync(int instanceId, string completedBy, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance is not active");
                }

                // Check if current step is final
                var currentStep = await _workflowStepRepository.GetByIdAsync(instance.CurrentStepId, tenantId);
                if (currentStep == null || !currentStep.IsFinal)
                {
                    return ServiceResult<bool>.FailureResult("Current step is not a final step");
                }

                _unitOfWork.BeginTransaction();

                instance.Status = WorkflowStatusConstants.Completed;
                instance.CompletedDate = DateTime.UtcNow;
                instance.CompletedBy = completedBy;

                await _workflowInstanceRepository.UpdateAsync(instance);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = instance.CurrentStepId,
                    ToStepId = 0, // Use 0 instead of null for non-nullable int
                    Action = "COMPLETE",
                    ActionCode = "COMPLETE",
                    ActionBy = completedBy,
                    TransitionDate = DateTime.UtcNow,
                    Comments = "Workflow completed"
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow completed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error completing workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelWorkflowAsync(int instanceId, string cancelledBy, int tenantId, string reason = null)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance is not active");
                }

                _unitOfWork.BeginTransaction();

                instance.Status = WorkflowStatusConstants.Cancelled;
                instance.CompletedDate = DateTime.UtcNow;
                instance.CompletedBy = cancelledBy;
                instance.Notes = string.IsNullOrEmpty(reason) ? "Workflow cancelled" : $"Cancelled: {reason}";

                await _workflowInstanceRepository.UpdateAsync(instance);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = instance.CurrentStepId,
                    ToStepId = 0, // Use 0 instead of null for non-nullable int
                    Action = "CANCEL",
                    ActionCode = "CANCEL",
                    ActionBy = cancelledBy,
                    TransitionDate = DateTime.UtcNow,
                    Comments = reason ?? "Workflow cancelled"
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow cancelled successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error cancelling workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AssignWorkflowAsync(int instanceId, string assignedTo, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Cannot assign inactive workflow");
                }

                _unitOfWork.BeginTransaction();

                instance.AssignedTo = assignedTo;

                await _workflowInstanceRepository.UpdateAsync(instance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow assigned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error assigning workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdatePriorityAsync(int instanceId, int priority, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (priority < 0)
                {
                    return ServiceResult<bool>.FailureResult("Priority cannot be negative");
                }

                _unitOfWork.BeginTransaction();

                instance.Priority = priority;

                await _workflowInstanceRepository.UpdateAsync(instance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Priority updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating priority: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateDueDateAsync(int instanceId, DateTime? dueDate, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                instance.DueDate = dueDate;

                await _workflowInstanceRepository.UpdateAsync(instance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Due date updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating due date: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateVariablesAsync(int instanceId, string variables, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                // Validate JSON format
                if (!string.IsNullOrEmpty(variables))
                {
                    try
                    {
                        JsonDocument.Parse(variables);
                    }
                    catch
                    {
                        return ServiceResult<bool>.FailureResult("Invalid JSON format for variables");
                    }
                }

                _unitOfWork.BeginTransaction();

                instance.Variables = variables;

                await _workflowInstanceRepository.UpdateAsync(instance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Variables updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating variables: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int instanceId, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                // Delete history entries first
                await _workflowHistoryRepository.DeleteByInstanceIdAsync(instanceId, tenantId);

                // Delete the instance
                bool result = await _workflowInstanceRepository.DeleteAsync(instanceId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow instance deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow instance: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(int tenantId)
        {
            return await _workflowInstanceRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<WorkflowInstance>>> GetPagedWorkflowInstancesAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var instances = await _workflowInstanceRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetInstancesByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            try
            {
                if (startDate > endDate)
                {
                    return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult("Start date must be before end date");
                }

                var instances = await _workflowInstanceRepository.GetByDateRangeAsync(startDate, endDate, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetActiveInstanceCountAsync(int workflowId, int tenantId)
        {
            try
            {
                var count = await _workflowInstanceRepository.GetActiveInstanceCountAsync(workflowId, tenantId);
                return ServiceResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.FailureResult($"Error retrieving active instance count: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetStuckInstancesAsync(int daysThreshold, int tenantId)
        {
            try
            {
                if (daysThreshold <= 0)
                {
                    return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult("Days threshold must be greater than zero");
                }

                var instances = await _workflowInstanceRepository.GetStuckInstancesAsync(daysThreshold, tenantId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving stuck instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RestartWorkflowAsync(int instanceId, string restartedBy, int tenantId)
        {
            try
            {
                var instance = await _workflowInstanceRepository.GetByIdAsync(instanceId, tenantId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status == WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance is already active");
                }

                // Get the initial step
                var initialStep = await _workflowStepRepository.GetInitialStepAsync(instance.WorkflowId, tenantId);
                if (initialStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow has no initial step defined");
                }

                _unitOfWork.BeginTransaction();

                // Reset instance to initial state
                instance.CurrentStepId = initialStep.StepId;
                instance.Status = WorkflowStatusConstants.Active;
                instance.CompletedDate = null;
                instance.CompletedBy = null;
                instance.LastTransitionDate = DateTime.UtcNow;
                instance.Notes = "Workflow restarted";

                await _workflowInstanceRepository.UpdateAsync(instance);

                // Create history entry
                var historyEntry = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = 0, // Use 0 instead of null for non-nullable int
                    ToStepId = initialStep.StepId,
                    Action = "RESTART",
                    ActionCode = "RESTART",
                    ActionBy = restartedBy,
                    TransitionDate = DateTime.UtcNow,
                    Comments = "Workflow restarted"
                };

                await _workflowHistoryRepository.InsertAsync(historyEntry);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow restarted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error restarting workflow: {ex.Message}");
            }
        }
    }
}
