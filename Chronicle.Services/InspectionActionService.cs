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
    public class InspectionActionService : IInspectionActionService
    {
        private readonly IInspectionActionRepository _inspectionActionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionActionService(
            IInspectionActionRepository inspectionActionRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionActionRepository = inspectionActionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionAction>> GetInspectionActionByIdAsync(int actionId)
        {
            try
            {
                var inspectionAction = await _inspectionActionRepository.GetByIdAsync(actionId);
                if (inspectionAction == null)
                {
                    return ServiceResult<InspectionAction>.FailureResult("Inspection action not found");
                }

                return ServiceResult<InspectionAction>.SuccessResult(inspectionAction);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionAction>.FailureResult($"Error retrieving inspection action: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectorUserIdAsync(int inspectorUserId)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByInspectorUserIdAsync(inspectorUserId);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByActionTypeAsync(string actionType)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByActionTypeAsync(actionType);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByInspectionResultAsync(string inspectionResult)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByInspectionResultAsync(inspectionResult);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByComplianceStatusAsync(string complianceStatus)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByComplianceStatusAsync(complianceStatus);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByActionDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByActionDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetPendingActionsAsync()
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetPendingActionsAsync();
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving pending inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetCompletedActionsAsync()
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetCompletedActionsAsync();
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving completed inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetActionsRequiringFollowUpAsync()
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetActionsRequiringFollowUpAsync();
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions requiring follow-up: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetActionsRequiringReviewAsync()
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetActionsRequiringReviewAsync();
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving inspection actions requiring review: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionAction>>> GetByReviewedByAsync(int reviewedBy)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetByReviewedByAsync(reviewedBy);
                return ServiceResult<IEnumerable<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionAction>>.FailureResult($"Error retrieving reviewed inspection actions: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionActionAsync(InspectionAction inspectionAction)
        {
            try
            {
                // Set default values
                inspectionAction.CreatedDate = DateTime.UtcNow;
                inspectionAction.ModifiedDate = DateTime.UtcNow;

                // Set default action date if not provided
                if (inspectionAction.ActionDate == default(DateTime))
                {
                    inspectionAction.ActionDate = DateTime.UtcNow;
                }

                _unitOfWork.BeginTransaction();

                int actionId = await _inspectionActionRepository.InsertAsync(inspectionAction);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(actionId, "Inspection action created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection action: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionAction inspectionAction)
        {
            try
            {
                // Check if inspection action exists
                var existingInspectionAction = await _inspectionActionRepository.GetByIdAsync(inspectionAction.ActionID);
                if (existingInspectionAction == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action not found");
                }

                inspectionAction.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionActionRepository.UpdateAsync(inspectionAction);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection action updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection action: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CompleteActionAsync(int actionId, string completionComments = null)
        {
            try
            {
                var existingAction = await _inspectionActionRepository.GetByIdAsync(actionId);
                if (existingAction == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action not found");
                }

                if (existingAction.IsCompleted)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action is already completed");
                }

                existingAction.IsCompleted = true;
                existingAction.CompletedDate = DateTime.UtcNow;
                existingAction.ModifiedDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(completionComments))
                {
                    existingAction.Comments = string.IsNullOrEmpty(existingAction.Comments)
                        ? completionComments
                        : $"{existingAction.Comments}\n\nCompletion: {completionComments}";
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionActionRepository.UpdateAsync(existingAction);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection action completed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error completing inspection action: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReviewActionAsync(int actionId, int reviewedBy, string reviewComments = null, bool approved = true)
        {
            try
            {
                var existingAction = await _inspectionActionRepository.GetByIdAsync(actionId);
                if (existingAction == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action not found");
                }

                if (existingAction.ReviewedBy.HasValue)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action has already been reviewed");
                }

                existingAction.ReviewedBy = reviewedBy;
                existingAction.ReviewedDate = DateTime.UtcNow;
                existingAction.ReviewComments = reviewComments;
                existingAction.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionActionRepository.UpdateAsync(existingAction);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection action reviewed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reviewing inspection action: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int actionId,int tenantId)
        {
            try
            {
                // Check if inspection action exists
                var existingInspectionAction = await _inspectionActionRepository.GetByIdAsync(actionId);
                if (existingInspectionAction == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection action not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionActionRepository.DeleteAsync(actionId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection action deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection action: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionAction>> GetInspectionActionsAsync()
        {
            return await _inspectionActionRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectionAction>>> GetPagedInspectionActionsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectionActions = await _inspectionActionRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectionAction>>.SuccessResult(inspectionActions);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionAction>>.FailureResult($"Error retrieving inspection actions: {ex.Message}");
            }
        }
    }
}
