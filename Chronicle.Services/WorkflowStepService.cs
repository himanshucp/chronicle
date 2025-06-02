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
        private readonly IWorkflowStepRepository _stepRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowStepService(
            IWorkflowStepRepository stepRepository,
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowStep>> GetStepByIdAsync(int stepId)
        {
            try
            {
                var step = await _stepRepository.GetByIdAsync(stepId);
                if (step == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Workflow step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(step);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetByWorkflowIdAsync(int workflowId)
        {
            try
            {
                var steps = await _stepRepository.GetByWorkflowIdAsync(workflowId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(steps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetInitialStepAsync(int workflowId)
        {
            try
            {
                var step = await _stepRepository.GetInitialStepAsync(workflowId);
                if (step == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Initial step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(step);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving initial step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowStep>>> GetFinalStepsAsync(int workflowId)
        {
            try
            {
                var steps = await _stepRepository.GetFinalStepsAsync(workflowId);
                return ServiceResult<IEnumerable<WorkflowStep>>.SuccessResult(steps);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowStep>>.FailureResult($"Error retrieving final steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowStep>> GetByStepCodeAsync(int workflowId, string stepCode)
        {
            try
            {
                var step = await _stepRepository.GetByStepCodeAsync(workflowId, stepCode);
                if (step == null)
                {
                    return ServiceResult<WorkflowStep>.FailureResult("Workflow step not found");
                }

                return ServiceResult<WorkflowStep>.SuccessResult(step);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowStep>.FailureResult($"Error retrieving workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<WorkflowStep>>> GetStepsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var steps = await _stepRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<WorkflowStep>>.SuccessResult(steps);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowStep>>.FailureResult($"Error retrieving workflow steps: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateStepAsync(WorkflowStep step)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(step.WorkflowId);
                if (workflow == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow not found");
                }

                var existingStep = await _stepRepository.GetByStepCodeAsync(step.WorkflowId, step.StepCode);
                if (existingStep != null)
                {
                    return ServiceResult<int>.FailureResult("Step code already exists in this workflow");
                }

                step.IsActive = true;

                _unitOfWork.BeginTransaction();

                int stepId = await _stepRepository.InsertAsync(step);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(stepId, "Workflow step created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowStep step)
        {
            try
            {
                var existingStep = await _stepRepository.GetByIdAsync(step.StepId);
                if (existingStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                var stepByCode = await _stepRepository.GetByStepCodeAsync(step.WorkflowId, step.StepCode);
                if (stepByCode != null && stepByCode.StepId != step.StepId)
                {
                    return ServiceResult<bool>.FailureResult("Step code already exists in this workflow");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _stepRepository.UpdateAsync(step);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow step: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int stepId,int tenantId)
        {
            try
            {
                var existingStep = await _stepRepository.GetByIdAsync(stepId);
                if (existingStep == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow step not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _stepRepository.DeleteAsync(stepId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow step deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow step: {ex.Message}");
            }
        }
    }
}
