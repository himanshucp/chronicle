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
    public class InspectionCommentService : IInspectionCommentService
    {
        private readonly IInspectionCommentRepository _inspectionCommentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionCommentService(
            IInspectionCommentRepository inspectionCommentRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionCommentRepository = inspectionCommentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionComment>> GetInspectionCommentByIdAsync(int commentId)
        {
            try
            {
                var inspectionComment = await _inspectionCommentRepository.GetByIdAsync(commentId);
                if (inspectionComment == null)
                {
                    return ServiceResult<InspectionComment>.FailureResult("Inspection comment not found");
                }

                return ServiceResult<InspectionComment>.SuccessResult(inspectionComment);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionComment>.FailureResult($"Error retrieving inspection comment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByInspectionRequestIdAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByInspectionRequestIdAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCreatedByAsync(int createdBy)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByCreatedByAsync(createdBy);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCommentTypeAsync(string commentType)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByCommentTypeAsync(commentType);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByPriorityAsync(string priority)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByPriorityAsync(priority);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByStatusAsync(string status)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByStatusAsync(status);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetInternalCommentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetInternalCommentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving internal inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetExternalCommentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetExternalCommentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving external inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetCommentsRequiringResponseAsync()
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetCommentsRequiringResponseAsync();
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving comments requiring response: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetOverdueCommentsAsync()
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetOverdueCommentsAsync();
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving overdue comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetRepliesAsync(int parentCommentId)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetRepliesAsync(parentCommentId);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving comment replies: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetRootCommentsAsync(int inspectionRequestId)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetRootCommentsAsync(inspectionRequestId);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving root comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByRespondedByAsync(int respondedBy)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByRespondedByAsync(respondedBy);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving responded comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionComment>>> GetByCreatedDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetByCreatedDateRangeAsync(fromDate, toDate);
                return ServiceResult<IEnumerable<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionCommentAsync(InspectionComment inspectionComment)
        {
            try
            {
                // Set default values
                inspectionComment.CreatedDate = DateTime.UtcNow;
                inspectionComment.ModifiedDate = DateTime.UtcNow;
                inspectionComment.IsActive = true;

                if (string.IsNullOrEmpty(inspectionComment.Status))
                {
                    inspectionComment.Status = "Open";
                }

                _unitOfWork.BeginTransaction();

                int commentId = await _inspectionCommentRepository.InsertAsync(inspectionComment);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(commentId, "Inspection comment created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection comment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> ReplyToCommentAsync(int parentCommentId, InspectionComment replyComment)
        {
            try
            {
                // Verify parent comment exists
                var parentComment = await _inspectionCommentRepository.GetByIdAsync(parentCommentId);
                if (parentComment == null)
                {
                    return ServiceResult<int>.FailureResult("Parent comment not found");
                }

                // Set the reply properties
                replyComment.ParentCommentID = parentCommentId;
                replyComment.InspectionRequestID = parentComment.InspectionRequestID;
                replyComment.CreatedDate = DateTime.UtcNow;
                replyComment.ModifiedDate = DateTime.UtcNow;
                replyComment.IsActive = true;

                if (string.IsNullOrEmpty(replyComment.Status))
                {
                    replyComment.Status = "Open";
                }

                _unitOfWork.BeginTransaction();

                int replyId = await _inspectionCommentRepository.InsertAsync(replyComment);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(replyId, "Reply created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating reply: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionComment inspectionComment)
        {
            try
            {
                // Check if inspection comment exists
                var existingInspectionComment = await _inspectionCommentRepository.GetByIdAsync(inspectionComment.CommentID);
                if (existingInspectionComment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection comment not found");
                }

                inspectionComment.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionCommentRepository.UpdateAsync(inspectionComment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection comment updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection comment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsRespondedAsync(int commentId, int respondedBy)
        {
            try
            {
                var existingComment = await _inspectionCommentRepository.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection comment not found");
                }

                if (!existingComment.RequiresResponse)
                {
                    return ServiceResult<bool>.FailureResult("Comment does not require a response");
                }

                if (existingComment.RespondedDate.HasValue)
                {
                    return ServiceResult<bool>.FailureResult("Comment has already been marked as responded");
                }

                existingComment.RespondedDate = DateTime.UtcNow;
                existingComment.RespondedBy = respondedBy;
                existingComment.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionCommentRepository.UpdateAsync(existingComment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Comment marked as responded successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error marking comment as responded: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CloseCommentAsync(int commentId, int closedBy)
        {
            try
            {
                var existingComment = await _inspectionCommentRepository.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection comment not found");
                }

                if (existingComment.Status == "Closed")
                {
                    return ServiceResult<bool>.FailureResult("Comment is already closed");
                }

                existingComment.Status = "Closed";
                existingComment.ModifiedDate = DateTime.UtcNow;
                existingComment.ModifiedBy = closedBy;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionCommentRepository.UpdateAsync(existingComment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Comment closed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error closing comment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReopenCommentAsync(int commentId, int reopenedBy)
        {
            try
            {
                var existingComment = await _inspectionCommentRepository.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection comment not found");
                }

                if (existingComment.Status != "Closed")
                {
                    return ServiceResult<bool>.FailureResult("Only closed comments can be reopened");
                }

                existingComment.Status = "Open";
                existingComment.ModifiedDate = DateTime.UtcNow;
                existingComment.ModifiedBy = reopenedBy;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionCommentRepository.UpdateAsync(existingComment);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Comment reopened successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error reopening comment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int commentId,int tenantId)
        {
            try
            {
                // Check if inspection comment exists
                var existingInspectionComment = await _inspectionCommentRepository.GetByIdAsync(commentId);
                if (existingInspectionComment == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection comment not found");
                }

                // Check if comment has replies
                var replies = await _inspectionCommentRepository.GetRepliesAsync(commentId);
                if (replies.Any())
                {
                    return ServiceResult<bool>.FailureResult("Cannot delete comment that has replies. Delete replies first or mark comment as inactive.");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionCommentRepository.DeleteAsync(commentId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection comment deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection comment: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionComment>> GetInspectionCommentsAsync()
        {
            return await _inspectionCommentRepository.GetAllAsync();
        }

        public async Task<ServiceResult<PagedResult<InspectionComment>>> GetPagedInspectionCommentsAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var inspectionComments = await _inspectionCommentRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<InspectionComment>>.SuccessResult(inspectionComments);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionComment>>.FailureResult($"Error retrieving inspection comments: {ex.Message}");
            }
        }
    }
}
