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
    public class InspectionRequestService : IInspectionRequestService
    {
        private readonly IInspectionRequestRepository _inspectionRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InspectionRequestService(
            IInspectionRequestRepository inspectionRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _inspectionRequestRepository = inspectionRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<InspectionRequest>> GetInspectionRequestByIdAsync(int inspectionRequestId, int tenantId)
        {
            try
            {
                var inspectionRequest = await _inspectionRequestRepository.GetByIdAsync(inspectionRequestId, tenantId);
                if (inspectionRequest == null)
                {
                    return ServiceResult<InspectionRequest>.FailureResult("Inspection request not found");
                }

                return ServiceResult<InspectionRequest>.SuccessResult(inspectionRequest);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionRequest>.FailureResult($"Error retrieving inspection request: {ex.Message}");
            }
        }

        public async Task<ServiceResult<InspectionRequest>> GetByIRNumberAsync(string irNumber, int tenantId)
        {
            try
            {
                var inspectionRequest = await _inspectionRequestRepository.GetByIRNumberAsync(irNumber, tenantId);
                if (inspectionRequest == null)
                {
                    return ServiceResult<InspectionRequest>.FailureResult("Inspection request not found");
                }

                return ServiceResult<InspectionRequest>.SuccessResult(inspectionRequest);
            }
            catch (Exception ex)
            {
                return ServiceResult<InspectionRequest>.FailureResult($"Error retrieving inspection request: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByContractIdAsync(int contractId, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByContractIdAsync(contractId, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByIssuerUserIdAsync(int issuerUserId, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByIssuerUserIdAsync(issuerUserId, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByIssuingCompanyIdAsync(int issuingCompanyId, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByIssuingCompanyIdAsync(issuingCompanyId, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByDisciplineIdAsync(int disciplineId, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByDisciplineIdAsync(disciplineId, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByInspectionDateRangeAsync(DateTime fromDate, DateTime toDate, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByInspectionDateRangeAsync(fromDate, toDate, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<InspectionRequest>>> GetByCurrentStatusAsync(string status, int tenantId)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetByCurrentStatusAsync(status, tenantId);
                return ServiceResult<IEnumerable<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> CreateInspectionRequestAsync(InspectionRequest inspectionRequest, int tenantId)
        {
            try
            {
                // Set tenant ID
                inspectionRequest.TenantID = tenantId;

                // Check if IR number already exists within the tenant
                if (!string.IsNullOrEmpty(inspectionRequest.IRNumber))
                {
                    var existingInspectionRequest = await _inspectionRequestRepository.GetByIRNumberAsync(inspectionRequest.IRNumber, tenantId);
                    if (existingInspectionRequest != null)
                    {
                        return ServiceResult<int>.FailureResult("Inspection request with this IR number already exists");
                    }
                }

                // Set default values
                inspectionRequest.CreatedDate = DateTime.UtcNow;
                inspectionRequest.ModifiedDate = DateTime.UtcNow;
                inspectionRequest.IsActive = true;

                _unitOfWork.BeginTransaction();

                int inspectionRequestId = await _inspectionRequestRepository.InsertAsync(inspectionRequest);

                _unitOfWork.Commit();

                return ServiceResult<int>.SuccessResult(inspectionRequestId, "Inspection request created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<int>.FailureResult($"Error creating inspection request: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(InspectionRequest inspectionRequest, int tenantId)
        {
            try
            {
                // Ensure tenant ID matches
                inspectionRequest.TenantID = tenantId;

                // Check if inspection request exists within the tenant
                var existingInspectionRequest = await _inspectionRequestRepository.GetByIdAsync(inspectionRequest.InspectionRequestID, tenantId);
                if (existingInspectionRequest == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection request not found");
                }

                // Check if IR number is unique within the tenant
                if (!string.IsNullOrEmpty(inspectionRequest.IRNumber))
                {
                    var inspectionRequestByIRNumber = await _inspectionRequestRepository.GetByIRNumberAsync(inspectionRequest.IRNumber, tenantId);
                    if (inspectionRequestByIRNumber != null && inspectionRequestByIRNumber.InspectionRequestID != inspectionRequest.InspectionRequestID)
                    {
                        return ServiceResult<bool>.FailureResult("Inspection request IR number already exists");
                    }
                }

                inspectionRequest.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionRequestRepository.UpdateAsync(inspectionRequest);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection request updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error updating inspection request: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int inspectionRequestId, int tenantId)
        {
            try
            {
                // Check if inspection request exists within the tenant
                var existingInspectionRequest = await _inspectionRequestRepository.GetByIdAsync(inspectionRequestId, tenantId);
                if (existingInspectionRequest == null)
                {
                    return ServiceResult<bool>.FailureResult("Inspection request not found");
                }

                _unitOfWork.BeginTransaction();

                bool result = await _inspectionRequestRepository.DeleteAsync(inspectionRequestId, tenantId);

                _unitOfWork.Commit();

                return ServiceResult<bool>.SuccessResult(result, "Inspection request deleted successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ServiceResult<bool>.FailureResult($"Error deleting inspection request: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionRequest>> GetInspectionRequestsAsync(int tenantId)
        {
            return await _inspectionRequestRepository.GetAllAsync(tenantId);
        }

        public async Task<ServiceResult<PagedResult<InspectionRequest>>> GetPagedInspectionRequestsAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            try
            {
                var inspectionRequests = await _inspectionRequestRepository.GetPagedAsync(page, pageSize, tenantId, searchTerm);
                return ServiceResult<PagedResult<InspectionRequest>>.SuccessResult(inspectionRequests);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<InspectionRequest>>.FailureResult($"Error retrieving inspection requests: {ex.Message}");
            }
        }
    }
}
