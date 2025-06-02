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
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        private readonly IWorkflowTransitionRepository _transitionRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowTransitionService(
            IWorkflowTransitionRepository transitionRepository,
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _transitionRepository = transitionRepository;
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowTransition>> GetTransitionByIdAsync(int transitionId)
        {
            try
            {
                var transition = await _transitionRepository.GetByIdAsync(transitionId);
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

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByWorkflowIdAsync(int workflowId)
        {
            try
            {
                var transitions = await _transitionRepository.GetByWorkflowIdAsync(workflowId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByFromStepIdAsync(int fromStepId)
        {
            try
            {
                var transitions = await _transitionRepository.GetByFromStepIdAsync(fromStepId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowTransition>>> GetByToStepIdAsync(int toStepId)
        {
            try
            {
                var transitions = await _transitionRepository.GetByToStepIdAsync(toStepId);
                return ServiceResult<IEnumerable<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowTransition>> GetByActionCodeAsync(int workflowId, string actionCode)
        {
            try
            {
                var transition = await _transitionRepository.GetByActionCodeAsync(workflowId, actionCode);
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

        public async Task<ServiceResult<PagedResult<WorkflowTransition>>> GetTransitionsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var transitions = await _transitionRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<WorkflowTransition>>.SuccessResult(transitions);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowTransition>>.FailureResult($"Error retrieving workflow transitions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateTransitionAsync(WorkflowTransition transition)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(transition.WorkflowId);
                if (workflow == null)
                {
                    return ServiceResult<int>.FailureResult("Workflow not found");
                }

                var existingTransition = await _transitionRepository.GetByActionCodeAsync(transition.WorkflowId, transition.ActionCode);
                if (existingTransition != null)
                {
                    return ServiceResult<int>.FailureResult("Action code already exists in this workflow");
                }

                transition.IsActive = true;

                _unitOfWork.BeginTransaction();

                int transitionId = await _transitionRepository.InsertAsync(transition);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(transitionId, "Workflow transition created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowTransition transition)
        {
            try
            {
                var existingTransition = await _transitionRepository.GetByIdAsync(transition.TransitionId);
                if (existingTransition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                var transitionByActionCode = await _transitionRepository.GetByActionCodeAsync(transition.WorkflowId, transition.ActionCode);
                if (transitionByActionCode != null && transitionByActionCode.TransitionId != transition.TransitionId)
                {
                    return ServiceResult<bool>.FailureResult("Action code already exists in this workflow");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _transitionRepository.UpdateAsync(transition);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow transition: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int transitionId,int tenantId)
        {
            try
            {
                var existingTransition = await _transitionRepository.GetByIdAsync(transitionId);
                if (existingTransition == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow transition not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _transitionRepository.DeleteAsync(transitionId,tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow transition deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow transition: {ex.Message}");
            }
        }
    }
}
