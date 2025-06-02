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
    public class SubDisciplineService : ISubDisciplineService
    {

        private readonly ISubDisciplineRepository _subDisciplineRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubDisciplineService(
            ISubDisciplineRepository subDisciplineRepository,
            IUnitOfWork unitOfWork)
        {
            _subDisciplineRepository = subDisciplineRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ActivateSubDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var subDiscipline = await _subDisciplineRepository.GetByIdAsync(id, tenantId);
                if (subDiscipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {id} not found");
                }

                subDiscipline.IsActive = true;
                subDiscipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _subDisciplineRepository.UpdateAsync(subDiscipline);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CreateSubDisciplineAsync(SubDiscipline subDiscipline, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure discipline belongs to the right tenant
                subDiscipline.TenantID = tenantId;

                // Validate discipline data
                if (string.IsNullOrEmpty(subDiscipline.SubDisciplineName))
                {
                    throw new ArgumentException("Discipline name is required");
                }

                // Check if discipline with same name already exists in this tenant
                var existingDiscipline = await _subDisciplineRepository.GetByNameAsync(subDiscipline.SubDisciplineName, tenantId);
                if (existingDiscipline != null)
                {
                    throw new InvalidOperationException($"A discipline with name '{subDiscipline.SubDisciplineName}' already exists");
                }

                // Set audit fields
                subDiscipline.CreatedDate = DateTime.UtcNow;
                subDiscipline.ModifiedDate = DateTime.UtcNow;
                subDiscipline.IsActive = true;

                int id = await _subDisciplineRepository.InsertAsync(subDiscipline);
                await _unitOfWork.CommitAsync();
                return id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeactivateSubDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var subDiscipline = await _subDisciplineRepository.GetByIdAsync(id, tenantId);
                if (subDiscipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {id} not found");
                }

                subDiscipline.IsActive = false;
                subDiscipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _subDisciplineRepository.UpdateAsync(subDiscipline);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
     
        public async Task<bool> DeleteSubDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var subDiscipline = await _subDisciplineRepository.GetByIdAsync(id, tenantId);
                if (subDiscipline == null)
                {
                    throw new InvalidOperationException($"SubDiscipline with ID {id} not found");
                }

                // Perform soft delete by setting IsActive to false
                subDiscipline.IsActive = false;
                subDiscipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _subDisciplineRepository.UpdateAsync(subDiscipline);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async  Task<IEnumerable<SubDiscipline>> GetActiveSubDisciplinesAsync(int tenantId)
        {
            return await _subDisciplineRepository.GetActiveAsync(tenantId);
        }

        public async Task<IEnumerable<SubDiscipline>> GetAllSubDisciplinesAsync(int tenantId)
        {
            return await _subDisciplineRepository.GetAllAsync(tenantId);
        }

        public async Task<PagedResult<SubDiscipline>> GetPagedSubDisciplinesAsync(int page, int pageSize, string searchTerm, int tenantId)
        {
            return await _subDisciplineRepository.GetPagedAsync(page, pageSize, searchTerm, tenantId);
        }

        public async Task<SubDiscipline> GetSubDisciplineByIdAsync(int id, int tenantId)
        {
            return await _subDisciplineRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<SubDiscipline> GetSubDisciplineByNameAsync(string name, int tenantId)
        {
            return await _subDisciplineRepository.GetByNameAsync(name, tenantId);
        }

        public async Task<bool> UpdateSubDisciplineAsync(SubDiscipline subDiscipline, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate discipline data
                if (string.IsNullOrEmpty(subDiscipline.SubDisciplineName))
                {
                    throw new ArgumentException("Discipline name is required");
                }

                // Get existing discipline and verify it belongs to this tenant
                var existingDiscipline = await _subDisciplineRepository.GetByIdAsync(subDiscipline.SubDisciplineID, tenantId);
                if (existingDiscipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {subDiscipline.SubDisciplineID} not found");
                }

                // Ensure tenant ID cannot be changed
                subDiscipline.TenantID = tenantId;

                // Check if another discipline with same name exists in this tenant
                var nameCheck = await _subDisciplineRepository.GetByNameAsync(subDiscipline.SubDisciplineName, tenantId);
                if (nameCheck != null && nameCheck.SubDisciplineID != subDiscipline.SubDisciplineID)
                {
                    throw new InvalidOperationException($"Another discipline with name '{subDiscipline.SubDisciplineName}' already exists");
                }

                // Set audit fields
                subDiscipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _subDisciplineRepository.UpdateAsync(subDiscipline);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

      
    }
}
