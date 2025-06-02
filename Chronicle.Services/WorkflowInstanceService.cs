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
    public class WorkflowInstanceService : IWorkflowInstanceService
    {
        private readonly IWorkflowInstanceRepository _instanceRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowStepRepository _stepRepository;
        private readonly IWorkflowTransitionRepository _transitionRepository;
        private readonly IWorkflowHistoryRepository _historyRepository;
        private readonly IWorkflowAssignmentRepository _assignmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowInstanceService(
            IWorkflowInstanceRepository instanceRepository,
            IWorkflowRepository workflowRepository,
            IWorkflowStepRepository stepRepository,
            IWorkflowTransitionRepository transitionRepository,
            IWorkflowHistoryRepository historyRepository,
            IWorkflowAssignmentRepository assignmentRepository,
            IUnitOfWork unitOfWork)
        {
            _instanceRepository = instanceRepository;
            _workflowRepository = workflowRepository;
            _stepRepository = stepRepository;
            _transitionRepository = transitionRepository;
            _historyRepository = historyRepository;
            _assignmentRepository = assignmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowInstance>> GetInstanceByIdAsync(int instanceId)
        {
            try
            {
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
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

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByWorkflowIdAsync(int workflowId)
        {
            try
            {
                var instances = await _instanceRepository.GetByWorkflowIdAsync(workflowId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByEntityAsync(string entityType, int entityId)
        {
            try
            {
                var instances = await _instanceRepository.GetByEntityAsync(entityType, entityId);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByStatusAsync(string status)
        {
            try
            {
                var instances = await _instanceRepository.GetByStatusAsync(status);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetByAssignedToAsync(string assignedTo)
        {
            try
            {
                var instances = await _instanceRepository.GetByAssignedToAsync(assignedTo);
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetActiveInstancesAsync()
        {
            try
            {
                var instances = await _instanceRepository.GetActiveInstancesAsync();
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving active instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowInstance>>> GetOverdueInstancesAsync()
        {
            try
            {
                var instances = await _instanceRepository.GetOverdueInstancesAsync();
                return ServiceResult<IEnumerable<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowInstance>>.FailureResult($"Error retrieving overdue instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<WorkflowInstance>>> GetInstancesAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var instances = await _instanceRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<WorkflowInstance>>.SuccessResult(instances);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowInstance>>.FailureResult($"Error retrieving workflow instances: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> StartWorkflowAsync(int workflowId, int entityId, string entityType, string startedBy)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId);
                if (workflow == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow not found");
                }

                if (!workflow.IsActive)
                {
                    return ServiceResult<int>.FailureResult("Workflow is not active");
                }

                var initialStep = await _stepRepository.GetInitialStepAsync(workflowId);
                if (initialStep == null)
                {
                    return ServiceResult<int>.FailureResult("Initial step not found for workflow");
                }

                _unitOfWork.BeginTransaction();

                var instance = new WorkflowInstance
                {
                    WorkflowId = workflowId,
                    EntityId = entityId,
                    EntityType = entityType,
                    CurrentStepId = initialStep.StepId,
                    Status = WorkflowStatusConstants.Active,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = startedBy,
                    Priority = WorkflowPriorityConstants.Normal
                };

                int instanceId = await _instanceRepository.InsertAsync(instance);

                // Create initial history entry
                var history = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = 0, // No previous step
                    ToStepId = initialStep.StepId,
                    ActionCode = "START",
                    Action = "Workflow Started",
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = startedBy
                };

                await _historyRepository.InsertAsync(history);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(instanceId, "Workflow started successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error starting workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> TransitionAsync(int instanceId, string actionCode, string actionBy, string comments = null)
        {
            try
            {
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
                if (instance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                if (instance.Status != WorkflowStatusConstants.Active)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance is not active");
                }

                var transitions = await _transitionRepository.GetByFromStepIdAsync(instance.CurrentStepId);
                var transition = transitions.FirstOrDefault(t => t.ActionCode == actionCode && t.IsActive);

                if (transition == null)
                {
                    return ServiceResult<bool>.FailureResult("Invalid transition");
                }

                var toStep = await _stepRepository.GetByIdAsync(transition.ToStepId);
                if (toStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Target step not found");
                }

                _unitOfWork.BeginTransaction();

                // Update instance
                var previousStepId = instance.CurrentStepId;
                instance.CurrentStepId = transition.ToStepId;
                instance.LastTransitionDate = DateTime.UtcNow;

                if (toStep.IsFinal)
                {
                    instance.Status = WorkflowStatusConstants.Completed;
                    instance.CompletedDate = DateTime.UtcNow;
                    instance.CompletedBy = actionBy;
                }

                await _instanceRepository.UpdateAsync(instance);

                // Create history entry
                var history = new WorkflowHistory
                {
                    InstanceId = instanceId,
                    FromStepId = previousStepId,
                    ToStepId = transition.ToStepId,
                    ActionCode = actionCode,
                    Action = transition.ActionName,
                    Comments = comments,
                    TransitionDate = DateTime.UtcNow,
                    ActionBy = actionBy
                };

                await _historyRepository.InsertAsync(history);

                // Complete previous assignments
                var activeAssignment = await _assignmentRepository.GetActiveAssignmentAsync(instanceId, previousStepId);
                if (activeAssignment != null)
                {
                    activeAssignment.CompletedDate = DateTime.UtcNow;
                    activeAssignment.IsActive = false;
                    await _assignmentRepository.UpdateAsync(activeAssignment);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow transitioned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error transitioning workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowInstance instance)
        {
            try
            {
                var existingInstance = await _instanceRepository.GetByIdAsync(instance.InstanceId);
                if (existingInstance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _instanceRepository.UpdateAsync(instance);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow instance updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow instance: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int instanceId,int tenantId)
        {
            try
            {
                var existingInstance = await _instanceRepository.GetByIdAsync(instanceId);
                if (existingInstance == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow instance not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _instanceRepository.DeleteAsync(instanceId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow instance deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow instance: {ex.Message}");
            }
        }
    }
}
