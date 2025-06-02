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
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowService(
            IWorkflowRepository workflowRepository,
            IUnitOfWork unitOfWork)
        {
            _workflowRepository = workflowRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<Workflow>> GetWorkflowByIdAsync(int workflowId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId);
                if (workflow == null)
                {
                    return ServiceResult<Workflow>.FailureResult("Workflow not found");
                }

                return ServiceResult<Workflow>.SuccessResult(workflow);
            }
            catch (Exception ex)
            {
                return ServiceResult<Workflow>.FailureResult($"Error retrieving workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Workflow>> GetByNameAsync(string workflowName)
        {
            try
            {
                var workflow = await _workflowRepository.GetByNameAsync(workflowName);
                if (workflow == null)
                {
                    return ServiceResult<Workflow>.FailureResult("Workflow not found");
                }

                return ServiceResult<Workflow>.SuccessResult(workflow);
            }
            catch (Exception ex)
            {
                return ServiceResult<Workflow>.FailureResult($"Error retrieving workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetByModuleAsync(string module)
        {
            try
            {
                var workflows = await _workflowRepository.GetByModuleAsync(module);
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetActiveWorkflowsAsync()
        {
            try
            {
                var workflows = await _workflowRepository.GetActiveWorkflowsAsync();
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<Workflow>>> GetWorkflowsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var workflows = await _workflowRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateWorkflowAsync(Workflow workflow)
        {
            try
            {
                var existingWorkflow = await _workflowRepository.GetByNameAsync(workflow.WorkflowName);
                if (existingWorkflow != null)
                {
                    return ServiceResult<int>.FailureResult("Workflow name already exists");
                }

                workflow.CreatedDate = DateTime.UtcNow;
                workflow.IsActive = true;
                workflow.Version = 1;
                workflow.LastModified = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                int workflowId = await _workflowRepository.InsertAsync(workflow);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(workflowId, "Workflow created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Workflow workflow)
        {
            try
            {
                var existingWorkflow = await _workflowRepository.GetByIdAsync(workflow.WorkflowId);
                if (existingWorkflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                var workflowByName = await _workflowRepository.GetByNameAsync(workflow.WorkflowName);
                if (workflowByName != null && workflowByName.WorkflowId != workflow.WorkflowId)
                {
                    return ServiceResult<bool>.FailureResult("Workflow name already exists");
                }

                workflow.LastModified = DateTime.UtcNow;
                workflow.Version = existingWorkflow.Version + 1;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowRepository.UpdateAsync(workflow);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int workflowId,int tenantId)
        {
            try
            {
                var existingWorkflow = await _workflowRepository.GetByIdAsync(workflowId);
                if (existingWorkflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _workflowRepository.DeleteAsync(workflowId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow: {ex.Message}");
            }
        }
    }
}
