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
    public class WorkflowHistoryService : IWorkflowHistoryService
    {
        private readonly IWorkflowHistoryRepository _historyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowHistoryService(
            IWorkflowHistoryRepository historyRepository,
            IUnitOfWork unitOfWork)
        {
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<WorkflowHistory>> GetHistoryByIdAsync(int historyId)
        {
            try
            {
                var history = await _historyRepository.GetByIdAsync(historyId);
                if (history == null)
                {
                    return ServiceResult<WorkflowHistory>.FailureResult("Workflow history not found");
                }

                return ServiceResult<WorkflowHistory>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkflowHistory>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetByInstanceIdAsync(int instanceId)
        {
            try
            {
                var history = await _historyRepository.GetByInstanceIdAsync(instanceId);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkflowHistory>>> GetByActionByAsync(string actionBy)
        {
            try
            {
                var history = await _historyRepository.GetByActionByAsync(actionBy);
                return ServiceResult<IEnumerable<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<WorkflowHistory>>> GetHistoryAsync(int page, int pageSize, string searchTerm = null)
        {
            try
            {
                var history = await _historyRepository.GetPagedAsync(page, pageSize, searchTerm);
                return ServiceResult<PagedResult<WorkflowHistory>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkflowHistory>>.FailureResult($"Error retrieving workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateHistoryAsync(WorkflowHistory history)
        {
            try
            {
                history.TransitionDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                int historyId = await _historyRepository.InsertAsync(history);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(historyId, "Workflow history created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(WorkflowHistory history)
        {
            try
            {
                var existingHistory = await _historyRepository.GetByIdAsync(history.HistoryId);
                if (existingHistory == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow history not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _historyRepository.UpdateAsync(history);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow history updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating workflow history: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int historyId,int tenantId)
        {
            try
            {
                var existingHistory = await _historyRepository.GetByIdAsync(historyId);
                if (existingHistory == null)
                {
                    return ServiceResult<bool>.FailureResult("Workflow history not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _historyRepository.DeleteAsync(historyId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Workflow history deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting workflow history: {ex.Message}");
            }
        }
    }

}
