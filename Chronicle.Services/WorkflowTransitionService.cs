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
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowStepRepository _workflowStepRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowTransitionService(
            IWorkflowTransitionRepository workflowTransitionRepository,
            IWorkflowRepository workflowRepository,
            IWorkflowStepRepository workflowStepRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowTransitionRepository = workflowTransitionRepository;
            _workflowRepository = workflowRepository;
            _workflowStepRepository = workflowStepRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowTransition>> GetWorkflowTransitionByIdAsync(int transitionId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<WorkflowTransition>.FailureResult("Workflow transition not found");
                }

                return ServiceResult<WorkflowTransition>.SuccessResult(transition);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowTransition>.FailureResult($"Error retrieving workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowTransition>> GetTransitionWithStepsAsync(int transitionId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByIdWithStepsAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<WorkflowTransition>.FailureResult("Workflow transition not found");
                }

                return ServiceResult<WorkflowTransition>.SuccessResult(transition);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowTransition>.FailureResult($"Error retrieving workflow transition with steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowTransition>> GetByActionCodeAsync(string actionCode, int fromStepId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByActionCodeAsync(actionCode, fromStepId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<WorkflowTransition>.FailureResult("Workflow transition not found");
                }

                return ServiceResult<WorkflowTransition>.SuccessResult(transition);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowTransition>.FailureResult($"Error retrieving workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsByWorkflowIdAsync(int workflowId, int tenantId)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetAvailableTransitionsAsync(int fromStepId, int tenantId, string userRole = null)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetByFromStepIdAsync(fromStepId, tenantId);

                // Filter by user role if provided
                if (!string.IsNullOrEmpty(userRole))
                {
                    transitions = transitions.Where(t =>
                    {
                        if (string.IsNullOrEmpty(t.AllowedRoles))
                            return true; // No role restriction

                        try
                        {
                            var allowedRoles = JsonSerializer.Deserialize<List<string>>(t.AllowedRoles);
                            return allowedRoles == null || allowedRoles.Contains(userRole);
                        }
                        catch
                        {
                            return true; // If parsing fails, allow transition
                        }
                    });
                }

                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving available transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetIncomingTransitionsAsync(int toStepId, int tenantId)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetByToStepIdAsync(toStepId, tenantId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving incoming transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetActiveTransitionsByWorkflowAsync(int workflowId, int tenantId)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetActiveTransitionsByWorkflowAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving active transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsByRoleAsync(string role, int workflowId, int tenantId)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetTransitionsByRoleAsync(role, workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving transitions by role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetTransitionsRequiringApprovalAsync(int workflowId, int tenantId)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetTransitionsRequiringApprovalAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving transitions requiring approval: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowTransition>> GetHighestPriorityTransitionAsync(int fromStepId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetHighestPriorityTransitionAsync(fromStepId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<WorkflowTransition>.FailureResult("No active transitions found from this step");
                }

                return ServiceResult<WorkflowTransition>.SuccessResult(transition);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowTransition>.FailureResult($"Error retrieving highest priority transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateWorkflowTransitionAsync(WorkflowTransition workflowTransition, int tenantId)
        {
            try
            {
                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowTransition.WorkflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow not found");
                }

                // Verify from step exists
                var fromStep = await _workflowStepRepository.GetByIdAsync(workflowTransition.FromStepId, tenantId);
                if (fromStep == null)
                {
                    return ServiceResult<int>.FailureResult("From step not found");
                }

                // Verify to step exists
                var toStep = await _workflowStepRepository.GetByIdAsync(workflowTransition.ToStepId, tenantId);
                if (toStep == null)
                {
                    return ServiceResult<int>.FailureResult("To step not found");
                }

                // Verify both steps belong to the same workflow
                if (fromStep.WorkflowId != workflowTransition.WorkflowId || toStep.WorkflowId != workflowTransition.WorkflowId)
                {
                    return ServiceResult<int>.FailureResult("Both steps must belong to the same workflow");
                }

                // Check if transition already exists
                var existingTransition = await _workflowTransitionRepository.TransitionExistsAsync(
                    workflowTransition.FromStepId,
                    workflowTransition.ToStepId,
                    workflowTransition.ActionCode,
                    tenantId);

                if (existingTransition)
                {
                    return ServiceResult<int>.FailureResult("A transition with this action code already exists between these steps");
                }

                // Validate transition logic
                var validationResult = await ValidateTransitionLogic(workflowTransition, fromStep, toStep);
                if (!validationResult.Success)
                {
                    return ServiceResult<int>.FailureResult(validationResult.Message);
                }

                // Set default values
                workflowTransition.IsActive = true;
                if (workflowTransition.Priority == 0)
                {
                    workflowTransition.Priority = 1;
                }

                _unitOfWork.BeginTransaction();

                int transitionId = await _workflowTransitionRepository.InsertAsync(workflowTransition);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(transitionId, "Workflow transition created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowTransition workflowTransition, int tenantId)
        {
            try
            {
                // Verify transition exists and belongs to tenant
                var existingTransition = await _workflowTransitionRepository.GetByIdAsync(workflowTransition.TransitionId, tenantId);
                if (existingTransition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowTransition.WorkflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Verify from step exists
                var fromStep = await _workflowStepRepository.GetByIdAsync(workflowTransition.FromStepId, tenantId);
                if (fromStep == null)
                {
                    return ServiceResult<bool>.FailureResult("From step not found");
                }

                // Verify to step exists
                var toStep = await _workflowStepRepository.GetByIdAsync(workflowTransition.ToStepId, tenantId);
                if (toStep == null)
                {
                    return ServiceResult<bool>.FailureResult("To step not found");
                }

                // Verify both steps belong to the same workflow
                if (fromStep.WorkflowId != workflowTransition.WorkflowId || toStep.WorkflowId != workflowTransition.WorkflowId)
                {
                    return ServiceResult<bool>.FailureResult("Both steps must belong to the same workflow");
                }

                // Check if transition already exists (excluding current transition)
                if (existingTransition.FromStepId != workflowTransition.FromStepId ||
                    existingTransition.ToStepId != workflowTransition.ToStepId ||
                    existingTransition.ActionCode != workflowTransition.ActionCode)
                {
                    var duplicateExists = await _workflowTransitionRepository.TransitionExistsAsync(
                        workflowTransition.FromStepId,
                        workflowTransition.ToStepId,
                        workflowTransition.ActionCode,
                        tenantId);

                    if (duplicateExists)
                    {
                        return ServiceResult<bool>.FailureResult("A transition with this action code already exists between these steps");
                    }
                }

                // Validate transition logic
                var validationResult = await ValidateTransitionLogic(workflowTransition, fromStep, toStep);
                if (!validationResult.Success)
                {
                    return ServiceResult<bool>.FailureResult(validationResult.Message);
                }

                // Preserve workflow ID from existing transition
                workflowTransition.WorkflowId = existingTransition.WorkflowId;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.UpdateAsync(workflowTransition);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int transitionId, int tenantId)
        {
            try
            {
                // Check if transition exists and belongs to tenant
                var existingTransition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (existingTransition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.DeleteAsync(transitionId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteByWorkflowIdAsync(int workflowId, int tenantId)
        {
            try
            {
                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.DeleteByWorkflowIdAsync(workflowId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "All workflow transitions deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow transitions: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkflowTransition>> GetWorkflowTransitionsAsync(int tenantId)
        {
            return await _workflowTransitionRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<WorkflowTransition>>> GetPagedWorkflowTransitionsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var transitions = await _workflowTransitionRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateTransitionAsync(int transitionId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                transition.IsActive = true;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.UpdateAsync(transition);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition activated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error activating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateTransitionAsync(int transitionId, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                transition.IsActive = false;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.UpdateAsync(transition);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateTransitionPriorityAsync(int transitionId, int newPriority, int tenantId)
        {
            try
            {
                var transition = await _workflowTransitionRepository.GetByIdAsync(transitionId, tenantId);
                if (transition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                if (newPriority < 0)
                {
                    return ServiceResult<bool>.FailureResult("Priority cannot be negative");
                }

                transition.Priority = newPriority;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowTransitionRepository.UpdateAsync(transition);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition priority updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow transition priority: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidateTransitionAsync(WorkflowTransition transition, int tenantId)
        {
            try
            {
                // Verify workflow exists
                var workflow = await _workflowRepository.GetByIdAsync(transition.WorkflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Verify from step exists
                var fromStep = await _workflowStepRepository.GetByIdAsync(transition.FromStepId, tenantId);
                if (fromStep == null)
                {
                    return ServiceResult<bool>.FailureResult("From step not found");
                }

                // Verify to step exists
                var toStep = await _workflowStepRepository.GetByIdAsync(transition.ToStepId, tenantId);
                if (toStep == null)
                {
                    return ServiceResult<bool>.FailureResult("To step not found");
                }

                // Validate transition logic
                var validationResult = await ValidateTransitionLogic(transition, fromStep, toStep);
                if (!validationResult.Success)
                {
                    return ServiceResult<bool>.FailureResult(validationResult.Message);
                }

                return ServiceResult<bool>.SuccessResult(true, "Transition is valid");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error validating transition: {ex.Message}");
            }
        }

        private async Task<ServiceResult<bool>> ValidateTransitionLogic(WorkflowTransition transition, WorkflowStep fromStep, WorkflowStep toStep)
        {
            // Validate that transition doesn't go from a final step
            if (fromStep.IsFinal)
            {
                return ServiceResult<bool>.FailureResult("Cannot create transition from a final step");
            }

            // Validate that transition doesn't go to an initial step
            if (toStep.IsInitial)
            {
                return ServiceResult<bool>.FailureResult("Cannot create transition to an initial step");
            }

            // Validate self-transition only if explicitly allowed
            if (transition.FromStepId == transition.ToStepId)
            {
                // You might want to add a flag to allow self-transitions in certain cases
                return ServiceResult<bool>.FailureResult("Self-transitions are not allowed");
            }

            // Validate JSON fields if provided
            if (!string.IsNullOrEmpty(transition.AllowedRoles))
            {
                try
                {
                    JsonSerializer.Deserialize<List<string>>(transition.AllowedRoles);
                }
                catch
                {
                    return ServiceResult<bool>.FailureResult("Invalid JSON format for AllowedRoles");
                }
            }

            if (!string.IsNullOrEmpty(transition.NotificationRoles))
            {
                try
                {
                    JsonSerializer.Deserialize<List<string>>(transition.NotificationRoles);
                }
                catch
                {
                    return ServiceResult<bool>.FailureResult("Invalid JSON format for NotificationRoles");
                }
            }

            if (!string.IsNullOrEmpty(transition.Condition))
            {
                try
                {
                    JsonDocument.Parse(transition.Condition);
                }
                catch
                {
                    return ServiceResult<bool>.FailureResult("Invalid JSON format for Condition");
                }
            }

            if (!string.IsNullOrEmpty(transition.Configuration))
            {
                try
                {
                    JsonDocument.Parse(transition.Configuration);
                }
                catch
                {
                    return ServiceResult<bool>.FailureResult("Invalid JSON format for Configuration");
                }
            }

            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}
