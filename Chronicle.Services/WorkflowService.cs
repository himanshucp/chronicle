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

        public async Task<ServiceResult<Workflow>> GetWorkflowByIdAsync(int workflowId, int tenantId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
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

        public async Task<ServiceResult<Workflow>> GetByNameAsync(string workflowName, int tenantId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByNameAsync(workflowName, tenantId);
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

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByModuleAsync(string module, int tenantId)
        {
            try
            {
                var workflows = await _workflowRepository.GetByModuleAsync(module, tenantId);
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetActiveWorkflowsAsync(int tenantId)
        {
            try
            {
                var workflows = await _workflowRepository.GetActiveWorkflowsAsync(tenantId);
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving active workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByCreatedByAsync(string createdBy, int tenantId)
        {
            try
            {
                var workflows = await _workflowRepository.GetByCreatedByAsync(createdBy, tenantId);
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Workflow>> GetByNameAndModuleAsync(string workflowName, string module, int tenantId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByNameAndModuleAsync(workflowName, module, tenantId);
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

        public async Task<ServiceResult<IEnumerable<Workflow>>> GetWorkflowsByVersionAsync(int version, int tenantId)
        {
            try
            {
                var workflows = await _workflowRepository.GetByVersionAsync(version, tenantId);
                return ServiceResult<IEnumerable<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateWorkflowAsync(Workflow workflow, int tenantId)
        {
            try
            {
                // Set tenant ID
                workflow.TenantID = tenantId;

                // Check if workflow name already exists within the tenant
                var existingWorkflowByName = await _workflowRepository.GetByNameAsync(workflow.WorkflowName, tenantId);
                if (existingWorkflowByName != null)
                {
                    return ServiceResult<int>.FailureResult("Workflow with this name already exists");
                }

                // Check if workflow with same name and module exists
                var existingWorkflowByNameAndModule = await _workflowRepository.GetByNameAndModuleAsync(workflow.WorkflowName, workflow.Module, tenantId);
                if (existingWorkflowByNameAndModule != null)
                {
                    return ServiceResult<int>.FailureResult("Workflow with this name and module combination already exists");
                }

                // Set default values
                workflow.CreatedDate = DateTime.UtcNow;
                workflow.Version = 1;
                workflow.IsActive = true;

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

        public async Task<ServiceResult<bool>> UpdateAsync(Workflow workflow, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                workflow.TenantID = tenantId;

                // Check if workflow exists within the tenant
                var existingWorkflow = await _workflowRepository.GetByIdAsync(workflow.WorkflowId, tenantId);
                if (existingWorkflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                // Check if workflow name is unique within the tenant (excluding current workflow)
                var workflowByName = await _workflowRepository.GetByNameAsync(workflow.WorkflowName, tenantId);
                if (workflowByName != null && workflowByName.WorkflowId != workflow.WorkflowId)
                {
                    return ServiceResult<bool>.FailureResult("Workflow name already exists");
                }

                // Check if workflow with same name and module exists (excluding current workflow)
                var workflowByNameAndModule = await _workflowRepository.GetByNameAndModuleAsync(workflow.WorkflowName, workflow.Module, tenantId);
                if (workflowByNameAndModule != null && workflowByNameAndModule.WorkflowId != workflow.WorkflowId)
                {
                    return ServiceResult<bool>.FailureResult("Workflow with this name and module combination already exists");
                }

                // Preserve created date and created by from existing workflow
                workflow.CreatedDate = existingWorkflow.CreatedDate;
                workflow.CreatedBy = existingWorkflow.CreatedBy;

                // Update modification info
                workflow.LastModified = DateTime.UtcNow;

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

        public async Task<ServiceResult<bool>> DeleteAsync(int workflowId, int tenantId)
        {
            try
            {
                // Check if workflow exists within the tenant
                var existingWorkflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
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

        public async Task<IEnumerable<Workflow>> GetWorkflowsAsync(int tenantId)
        {
            return await _workflowRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<Workflow>>> GetPagedWorkflowsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var workflows = await _workflowRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<Workflow>>.SuccessResult(workflows);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<Workflow>>.FailureResult($"Error retrieving workflows: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateWorkflowAsync(int workflowId, int tenantId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                workflow.IsActive = false;
                workflow.LastModified = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowRepository.UpdateAsync(workflow);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow deactivated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deactivating workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateWorkflowAsync(int workflowId, int tenantId)
        {
            try
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (workflow == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow not found");
                }

                workflow.IsActive = true;
                workflow.LastModified = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _workflowRepository.UpdateAsync(workflow);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow activated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error activating workflow: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Workflow>> CloneWorkflowAsync(int workflowId, string newWorkflowName, int tenantId, string createdBy)
        {
            try
            {
                // Get the original workflow
                var originalWorkflow = await _workflowRepository.GetByIdAsync(workflowId, tenantId);
                if (originalWorkflow == null)
                {
                    return ServiceResult<Workflow>.FailureResult("Original workflow not found");
                }

                // Check if new workflow name already exists
                var existingWorkflow = await _workflowRepository.GetByNameAsync(newWorkflowName, tenantId);
                if (existingWorkflow != null)
                {
                    return ServiceResult<Workflow>.FailureResult("Workflow with this name already exists");
                }

                // Create a clone
                var clonedWorkflow = new Workflow
                {
                    TenantID = tenantId,
                    WorkflowName = newWorkflowName,
                    Description = originalWorkflow.Description,
                    Module = originalWorkflow.Module,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = createdBy,
                    Configuration = originalWorkflow.Configuration,
                    Version = 1,
                    LastModified = null,
                    LastModifiedBy = null
                };

                _unitOfWork.BeginTransaction();

                int clonedWorkflowId = await _workflowRepository.InsertAsync(clonedWorkflow);
                clonedWorkflow.WorkflowId = clonedWorkflowId;

                _unitOfWork.Commit();

                return ServiceResult<Workflow>.SuccessResult(clonedWorkflow, "Workflow cloned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<Workflow>.FailureResult($"Error cloning workflow: {ex.Message}");
            }
        }
    }
}
