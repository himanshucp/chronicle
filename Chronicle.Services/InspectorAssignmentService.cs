using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using Chronicle.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class InspectorAssignmentService : IInspectorAssignmentService
    {
        private readonly IInspectorAssignmentRepository _inspectorAssignmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectorAssignmentService(
            IInspectorAssignmentRepository inspectorAssignmentRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectorAssignmentRepository = inspectorAssignmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectorAssignment>> GetInspectorAssignmentByIdAsync(int assignmentId)
        {
            try
            {
                var inspectorAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (inspectorAssignment == null)
                {
                    return ServiceResult<InspectorAssignment>.FailureResult("Inspector assignment not found");
                }

                return ServiceResult<InspectorAssignment>.SuccessResult(inspectorAssignment);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectorAssignment>.FailureResult($"Error retrieving inspector assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByInspectorUserIdAsync(int inspectorUserId)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByInspectorUserIdAsync(inspectorUserId);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignedByAsync(int assignedBy)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByAssignedByAsync(assignedBy);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignmentTypeAsync(string assignmentType)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByAssignmentTypeAsync(assignmentType);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByStatusAsync(string status)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByStatusAsync(status);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetByAssignedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetByAssignedDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetPendingAssignmentsAsync()
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetPendingAssignmentsAsync();
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving pending inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetAcceptedAssignmentsAsync()
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetAcceptedAssignmentsAsync();
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving accepted inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetDeclinedAssignmentsAsync()
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetDeclinedAssignmentsAsync();
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving declined inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetCompletedAssignmentsAsync()
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetCompletedAssignmentsAsync();
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving completed inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetOverdueAssignmentsAsync()
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetOverdueAssignmentsAsync();
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving overdue inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectorAssignment>>> GetActiveAssignmentsForInspectorAsync(int inspectorUserId)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetActiveAssignmentsForInspectorAsync(inspectorUserId);
                return ServiceResult<IEnumerable<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectorAssignment>>.FailureResult($"Error retrieving active inspector assignments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<InspectorAssignment>> GetActiveAssignmentForInspectionRequestAsync(int inspectionRequestId)
        {
            try
            {
                var inspectorAssignment = await _inspectorAssignmentRepository.GetActiveAssignmentForInspectionRequestAsync(inspectionRequestId);
                if (inspectorAssignment == null)
                {
                    return ServiceResult<InspectorAssignment>.FailureResult("No active assignment found for this inspection request");
                }

                return ServiceResult<InspectorAssignment>.SuccessResult(inspectorAssignment);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectorAssignment>.FailureResult($"Error retrieving active inspector assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectorAssignmentAsync(InspectorAssignment inspectorAssignment)
        {
            try
            {
                // Check if there's already an active assignment for this inspection request
                var activeAssignment = await _inspectorAssignmentRepository.GetActiveAssignmentForInspectionRequestAsync(inspectorAssignment.InspectionRequestID);
                if (activeAssignment != null)
                {
                    return ServiceResult<int>.FailureResult("There is already an active assignment for this inspection request");
                }

                // Set default values
                inspectorAssignment.CreatedDate = DateTime.UtcNow;
                inspectorAssignment.ModifiedDate = DateTime.UtcNow;
                inspectorAssignment.IsActive = true;

                if (inspectorAssignment.AssignedDate == default(DateTime))
                {
                    inspectorAssignment.AssignedDate = DateTime.UtcNow;
                }

                if (string.IsNullOrEmpty(inspectorAssignment.Status))
                {
                    inspectorAssignment.Status = "Pending";
                }

                _unitOfWork.BeginTransaction();

                int assignmentId = await _inspectorAssignmentRepository.InsertAsync(inspectorAssignment);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(assignmentId, "Inspector assignment created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspector assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectorAssignment inspectorAssignment)
        {
            try
            {
                // Check if inspector assignment exists
                var existingInspectorAssignment = await _inspectorAssignmentRepository.GetByIdAsync(inspectorAssignment.AssignmentID);
                if (existingInspectorAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                inspectorAssignment.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.UpdateAsync(inspectorAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspector assignment updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspector assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AcceptAssignmentAsync(int assignmentId, string notes = null)
        {
            try
            {
                var existingAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                if (existingAssignment.Status != "Pending")
                {
                    return ServiceResult<bool>.FailureResult("Only pending assignments can be accepted");
                }

                existingAssignment.Status = "Accepted";
                existingAssignment.AcceptanceDate = DateTime.UtcNow;
                existingAssignment.ModifiedDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(notes))
                {
                    existingAssignment.Notes = string.IsNullOrEmpty(existingAssignment.Notes)
                        ? notes
                        : $"{existingAssignment.Notes}\n\nAcceptance: {notes}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.UpdateAsync(existingAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment accepted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error accepting assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeclineAssignmentAsync(int assignmentId, string declineReason, string notes = null)
        {
            try
            {
                var existingAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                if (existingAssignment.Status != "Pending")
                {
                    return ServiceResult<bool>.FailureResult("Only pending assignments can be declined");
                }

                existingAssignment.Status = "Declined";
                existingAssignment.DeclineReason = declineReason;
                existingAssignment.ModifiedDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(notes))
                {
                    existingAssignment.Notes = string.IsNullOrEmpty(existingAssignment.Notes)
                        ? notes
                        : $"{existingAssignment.Notes}\n\nDecline: {notes}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.UpdateAsync(existingAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment declined successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error declining assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> StartAssignmentAsync(int assignmentId)
        {
            try
            {
                var existingAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                if (existingAssignment.Status != "Accepted")
                {
                    return ServiceResult<bool>.FailureResult("Only accepted assignments can be started");
                }

                existingAssignment.Status = "In Progress";
                existingAssignment.ActualStartDate = DateTime.UtcNow;
                existingAssignment.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.UpdateAsync(existingAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment started successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error starting assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CompleteAssignmentAsync(int assignmentId, string notes = null)
        {
            try
            {
                var existingAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                if (existingAssignment.Status == "Completed")
                {
                    return ServiceResult<bool>.FailureResult("Assignment is already completed");
                }

                if (existingAssignment.Status != "In Progress" && existingAssignment.Status != "Accepted")
                {
                    return ServiceResult<bool>.FailureResult("Only in-progress or accepted assignments can be completed");
                }

                existingAssignment.Status = "Completed";
                existingAssignment.ActualCompletionDate = DateTime.UtcNow;
                existingAssignment.ModifiedDate = DateTime.UtcNow;

                // Set actual start date if not already set
                if (!existingAssignment.ActualStartDate.HasValue)
                {
                    existingAssignment.ActualStartDate = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(notes))
                {
                    existingAssignment.Notes = string.IsNullOrEmpty(existingAssignment.Notes)
                        ? notes
                        : $"{existingAssignment.Notes}\n\nCompletion: {notes}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.UpdateAsync(existingAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Assignment completed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error completing assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReassignAsync(int assignmentId, int newInspectorUserId, int reassignedBy, string reason = null)
        {
            try
            {
                var existingAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                if (existingAssignment.Status == "Completed")
                {
                    return ServiceResult<bool>.FailureResult("Completed assignments cannot be reassigned");
                }

                // Deactivate the current assignment
                existingAssignment.IsActive = false;
                existingAssignment.ModifiedDate = DateTime.UtcNow;

                // Create a new assignment
                var newAssignment = new InspectorAssignment
                {
                    InspectionRequestID = existingAssignment.InspectionRequestID,
                    InspectorUserID = newInspectorUserId,
                    AssignmentType = existingAssignment.AssignmentType,
                    AssignedDate = DateTime.UtcNow,
                    AssignedBy = reassignedBy,
                    ExpectedStartDate = existingAssignment.ExpectedStartDate,
                    ExpectedCompletionDate = existingAssignment.ExpectedCompletionDate,
                    Status = "Pending",
                    Notes = string.IsNullOrEmpty(reason) ? "Reassigned from another inspector" : $"Reassigned: {reason}",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                _unitOfWork.BeginTransaction();

                // Update the old assignment
                await _inspectorAssignmentRepository.UpdateAsync(existingAssignment);

                // Create the new assignment
                int newAssignmentId = await _inspectorAssignmentRepository.InsertAsync(newAssignment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(true, "Assignment reassigned successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reassigning assignment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int assignmentId,int tenantId)
        {
            try
            {
                // Check if inspector assignment exists
                var existingInspectorAssignment = await _inspectorAssignmentRepository.GetByIdAsync(assignmentId);
                if (existingInspectorAssignment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspector assignment not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectorAssignmentRepository.DeleteAsync(assignmentId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspector assignment deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspector assignment: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectorAssignment>> GetInspectorAssignmentsAsync()
        {
            return await _inspectorAssignmentRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectorAssignment>>> GetPagedInspectorAssignmentsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectorAssignments = await _inspectorAssignmentRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectorAssignment>>.SuccessResult(inspectorAssignments);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectorAssignment>>.FailureResult($"Error retrieving inspector assignments: {ex.Message}");
            }
        }

       
    }
}
