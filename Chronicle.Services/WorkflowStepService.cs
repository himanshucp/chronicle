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
    public class WorkflowStepService : IWorkflowStepService
    {
        private readonly IWorkflowStepRepository _workflowStepRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowStepService(
            IWorkflowStepRepository workflowStepRepository,
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowStepRepository = workflowStepRepository;
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowStep>> GetWorkflowStepByIdAsync(int stepId, int tenantId)
        {
            try
            {
                var workflowStep = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (workflowStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Workflow step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(workflowStep);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetWorkflowStepWithTransitionsAsync(int stepId, int tenantId)
        {
            try
            {
                var workflowStep = await _workflowStepRepository.GetByIdWithTransitionsAsync(stepId, tenantId);
                if (workflowStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Workflow step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(workflowStep);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving workflow step with transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetByStepCodeAsync(string stepCode, int workflowId, int tenantId)
        {
            try
            {
                var workflowStep = await _workflowStepRepository.GetByStepCodeAsync(stepCode, workflowId, tenantId);
                if (workflowStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Workflow step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(workflowStep);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByWorkflowIdAsync(int workflowId, int tenantId)
        {
            try
            {
                var workflowSteps = await _workflowStepRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(workflowSteps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByTypeAsync(string stepType, int tenantId)
        {
            try
            {
                var workflowSteps = await _workflowStepRepository.GetByStepTypeAsync(stepType, tenantId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(workflowSteps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetInitialStepAsync(int workflowId, int tenantId)
        {
            try
            {
                var initialStep = await _workflowStepRepository.GetInitialStepAsync(workflowId, tenantId);
                if (initialStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("No initial step found for this workflow");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(initialStep);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving initial step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetFinalStepsAsync(int workflowId, int tenantId)
        {
            try
            {
                var finalSteps = await _workflowStepRepository.GetFinalStepsAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(finalSteps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving final steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetNextStepAsync(int currentStepId, int tenantId)
        {
            try
            {
                var nextStep = await _workflowStepRepository.GetNextStepAsync(currentStepId, tenantId);
                if (nextStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("No next step defined");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(nextStep);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving next step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetActiveStepsByWorkflowAsync(int workflowId, int tenantId)
        {
            try
            {
                var activeSteps = await _workflowStepRepository.GetActiveStepsByWorkflowAsync(workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(activeSteps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving active steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetStepsByRoleAsync(string role, int workflowId, int tenantId)
        {
            try
            {
                var steps = await _workflowStepRepository.GetStepsByRoleAsync(role, workflowId, tenantId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(steps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving steps by role: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateWorkflowStepAsync(WorkflowStep workflowStep, int tenantId)
        {
            try
            {
                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowStep.WorkflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow not found");
                }

                // Check if step code already exists within the workflow
                var existingStep = await _workflowStepRepository.GetByStepCodeAsync(workflowStep.StepCode, workflowStep.WorkflowId, tenantId);
                if (existingStep != null)
                {
                    return ServiceResult<int>.FailureResult("Step code already exists in this workflow");
                }

                // Validate workflow step logic
                if (workflowStep.IsInitial && workflowStep.IsFinal)
                {
                    return ServiceResult<int>.FailureResult("A step cannot be both initial and final");
                }

                // Check if another initial step exists
                if (workflowStep.IsInitial)
                {
                    var existingInitialStep = await _workflowStepRepository.GetInitialStepAsync(workflowStep.WorkflowId, tenantId);
                    if (existingInitialStep != null)
                    {
                        return ServiceResult<int>.FailureResult("Workflow already has an initial step");
                    }
                }

                // Set default values
                if (workflowStep.StepOrder == 0)
                {
                    var maxOrder = await _workflowStepRepository.GetMaxStepOrderAsync(workflowStep.WorkflowId, tenantId);
                    workflowStep.StepOrder = maxOrder + 10; // Leave gaps for easier reordering
                }

                _unitOfWork.BeginTransaction();

                int stepId = await _workflowStepRepository.InsertAsync(workflowStep);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(stepId, "Workflow step created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowStep workflowStep, int tenantId)
        {
            try
            {
                // Verify step exists and belongs to tenant
                var existingStep = await _workflowStepRepository.GetByIdAsync(workflowStep.StepId, tenantId);
                if (existingStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowStep.WorkflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Check if step code is unique within the workflow (excluding current step)
                var stepByCode = await _workflowStepRepository.GetByStepCodeAsync(workflowStep.StepCode, workflowStep.WorkflowId, tenantId);
                if (stepByCode != null && stepByCode.StepId != workflowStep.StepId)
                {
                    return ServiceResult<bool>.FailureResult("Step code already exists in this workflow");
                }

                // Validate workflow step logic
                if (workflowStep.IsInitial && workflowStep.IsFinal)
                {
                    return ServiceResult<bool>.FailureResult("A step cannot be both initial and final");
                }

                // Check if another initial step exists (excluding current step)
                if (workflowStep.IsInitial && !existingStep.IsInitial)
                {
                    var existingInitialStep = await _workflowStepRepository.GetInitialStepAsync(workflowStep.WorkflowId, tenantId);
                    if (existingInitialStep != null)
                    {
                        return ServiceResult<bool>.FailureResult("Workflow already has an initial step");
                    }
                }

                // Preserve workflow ID from existing step
                workflowStep.WorkflowId = existingStep.WorkflowId;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowStepRepository.UpdateAsync(workflowStep);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int stepId, int tenantId)
        {
            try
            {
                // Check if step exists and belongs to tenant
                var existingStep = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (existingStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowStepRepository.DeleteAsync(stepId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow step: {ex.Message}");
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

                bool result = await _workflowStepRepository.DeleteByWorkflowIdAsync(workflowId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "All workflow steps deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow steps: {ex.Message}");
            }
        }

        public async Task<IEnumerable<WorkflowStep>> GetWorkflowStepsAsync(int tenantId)
        {
            return await _workflowStepRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<WorkflowStep>>> GetPagedWorkflowStepsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var workflowSteps = await _workflowStepRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<WorkflowStep>>.SuccessResult(workflowSteps);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowStep>>.FailureResult($"Error retrieving workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReorderStepsAsync(int workflowId, List<int> stepIds, int tenantId)
        {
            try
            {
                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Get all steps for the workflow
                var allSteps = await _workflowStepRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                var allStepsList = allSteps.ToList();

                // Verify all step IDs are valid and belong to the workflow
                if (stepIds.Count != allStepsList.Count || !stepIds.All(id => allStepsList.Any(s => s.StepId == id)))
                {
                    return ServiceResult<bool>.FailureResult("Invalid step IDs provided");
                }

                _unitOfWork.BeginTransaction();

                // Update step orders
                for (int i = 0; i < stepIds.Count; i++)
                {
                    var step = allStepsList.First(s => s.StepId == stepIds[i]);
                    step.StepOrder = (i + 1) * 10; // Leave gaps for easier future reordering
                    await _workflowStepRepository.UpdateAsync(step);
                }

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Workflow steps reordered successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reordering workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateStepAsync(int stepId, int tenantId)
        {
            try
            {
                var step = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (step == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                step.IsActive = true;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowStepRepository.UpdateAsync(step);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step activated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error activating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateStepAsync(int stepId, int tenantId)
        {
            try
            {
                var step = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (step == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                step.IsActive = false;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowStepRepository.UpdateAsync(step);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> CloneStepAsync(int stepId, int targetWorkflowId, int tenantId)
        {
            try
            {
                // Get the original step
                var originalStep = await _workflowStepRepository.GetByIdAsync(stepId, tenantId);
                if (originalStep == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Original workflow step not found");
                }

                // Verify target workflow exists and belongs to tenant
                var targetWorkflow = await _workflowRepository.GetByIdAsync(targetWorkflowId, tenantId);
                if (targetWorkflow == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Target workflow not found");
                }

                // Generate new step code if it conflicts
                string newStepCode = originalStep.StepCode;
                int suffix = 1;
                while (await _workflowStepRepository.GetByStepCodeAsync(newStepCode, targetWorkflowId, tenantId) != null)
                {
                    newStepCode = $"{originalStep.StepCode}_COPY{suffix}";
                    suffix++;
                }

                // Create a clone
                var clonedStep = new WorkflowStep
                {
                    WorkflowId = targetWorkflowId,
                    StepCode = newStepCode,
                    StepName = originalStep.StepName,
                    StepType = originalStep.StepType,
                    StepOrder = await _workflowStepRepository.GetMaxStepOrderAsync(targetWorkflowId, tenantId) + 10,
                    NextStepId = null, // Don't copy next step reference as it might not exist in target workflow
                    IsFinal = originalStep.IsFinal,
                    IsInitial = false, // Don't copy initial flag to avoid conflicts
                    AllowedRoles = originalStep.AllowedRoles,
                    Conditions = originalStep.Conditions,
                    Configuration = originalStep.Configuration,
                    IsActive = true,
                    TimeoutHours = originalStep.TimeoutHours,
                    RequiresComments = originalStep.RequiresComments,
                    AllowDelegation = originalStep.AllowDelegation
                };

                _unitOfWork.BeginTransaction();

                int clonedStepId = await _workflowStepRepository.InsertAsync(clonedStep);
                clonedStep.StepId = clonedStepId;

                _unitOfWork.Commit();

                return ServiceResult<WorkflowStep>.SuccessResult(clonedStep, "Workflow step cloned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<WorkflowStep>.FailureResult($"Error cloning workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidateWorkflowStepsAsync(int workflowId, int tenantId)
        {
            try
            {
                // Verify workflow exists and belongs to tenant
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Get all steps for the workflow
                var allSteps = await _workflowStepRepository.GetByWorkflowIdAsync(workflowId, tenantId);
                var stepsList = allSteps.ToList();

                if (!stepsList.Any())
                {
                    return ServiceResult<bool>.FailureResult("Workflow has no steps defined");
                }

                // Check for initial step
                var initialSteps = stepsList.Where(s => s.IsInitial).ToList();
                if (initialSteps.Count == 0)
                {
                    return ServiceResult<bool>.FailureResult("Workflow must have an initial step");
                }
                if (initialSteps.Count > 1)
                {
                    return ServiceResult<bool>.FailureResult("Workflow cannot have more than one initial step");
                }

                // Check for at least one final step
                var finalSteps = stepsList.Where(s => s.IsFinal).ToList();
                if (!finalSteps.Any())
                {
                    return ServiceResult<bool>.FailureResult("Workflow must have at least one final step");
                }

                // Check for duplicate step codes
                var duplicateCodes = stepsList.GroupBy(s => s.StepCode)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateCodes.Any())
                {
                    return ServiceResult<bool>.FailureResult($"Duplicate step codes found: {string.Join(", ", duplicateCodes)}");
                }

                // Validate NextStepId references
                var stepIds = stepsList.Select(s => s.StepId).ToHashSet();
                var invalidReferences = stepsList
                    .Where(s => s.NextStepId.HasValue && !stepIds.Contains(s.NextStepId.Value))
                    .ToList();

                if (invalidReferences.Any())
                {
                    return ServiceResult<bool>.FailureResult("Some steps reference non-existent next steps");
                }

                // Check for circular references
                foreach (var step in stepsList.Where(s => s.NextStepId.HasValue))
                {
                    var visited = new HashSet<int>();
                    var current = step;

                    while (current != null && current.NextStepId.HasValue)
                    {
                        if (visited.Contains(current.StepId))
                        {
                            return ServiceResult<bool>.FailureResult($"Circular reference detected starting from step: {step.StepName}");
                        }

                        visited.Add(current.StepId);
                        current = stepsList.FirstOrDefault(s => s.StepId == current.NextStepId.Value);
                    }
                }

                return ServiceResult<bool>.SuccessResult(true, "Workflow steps are valid");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult($"Error validating workflow steps: {ex.Message}");
            }
        }
    }
}
