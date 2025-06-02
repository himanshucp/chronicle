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
        private readonly IWorkflowAssignmentRepository _assignmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowAssignmentService(
            IWorkflowAssignmentRepository assignmentRepository,
            IUnitOfWork unitOfWork)
        {
            _assignmentRepository = assignmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowAssignment>> GetAssignmentByIdAsync(int assignmentId)
        {
            try
            {
                var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
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

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetByInstanceIdAsync(int instanceId)
        {
            try
            {
                var assignments = await _assignmentRepository.GetByInstanceIdAsync(instanceId);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving workflow assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetByAssignedToAsync(string assignedTo)
        {
            try
            {
                var assignments = await _assignmentRepository.GetByAssignedToAsync(assignedTo);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving workflow assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowAssignment>>> GetActiveAssignmentsAsync(string assignedTo)
        {
            try
            {
                var assignments = await _assignmentRepository.GetActiveAssignmentsAsync(assignedTo);
                return ServiceResult<IEnumerable<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowAssignment>>.FailureResult($"Error retrieving active assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WorkflowAssignment>> GetActiveAssignmentAsync(int instanceId, int stepId)
        {
            try
            {
                var assignment = await _assignmentRepository.GetActiveAssignmentAsync(instanceId, stepId);
                if (assignment == null)
                {
                    return ServiceResult<WorkflowAssignment>.FailureResult("Active assignment not found");
                }

                return ServiceResult<WorkflowAssignment>.SuccessResult(assignment);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowAssignment>.FailureResult($"Error retrieving active assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<WorkflowAssignment>>> GetAssignmentsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var assignments = await _assignmentRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<WorkflowAssignment>>.SuccessResult(assignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowAssignment>>.FailureResult($"Error retrieving workflow assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateAssignmentAsync(WorkflowAssignment assignment)
        {
            try
            {
                assignment.AssignedDate = DateTime.UtcNow;
                assignment.IsActive = true;

                _unitOfWork.BeginTransaction();

                int assignmentId = await _assignmentRepository.InsertAsync(assignment);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(assignmentId, "Workflow assignment created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, string completedBy)
        {
            try
            {
                var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
                if (assignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                assignment.CompletedDate = DateTime.UtcNow;
                assignment.IsActive = false;

                _unitOfWork.BeginTransaction();

                bool result = await _assignmentRepository.UpdateAsync(assignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment completed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error completing assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowAssignment assignment)
        {
            try
            {
                var existingAssignment = await _assignmentRepository.GetByIdAsync(assignment.AssignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _assignmentRepository.UpdateAsync(assignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow assignment updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int assignmentId,int tenantId)
        {
            try
            {
                var existingAssignment = await _assignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow assignment not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _assignmentRepository.DeleteAsync(assignmentId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow assignment deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow assignment: {ex.Message}");
            }
        }

     
    }
}
