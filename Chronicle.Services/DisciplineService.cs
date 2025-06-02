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
    public class DisciplineService : IDisciplineService
    {
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DisciplineService(
            IDisciplineRepository disciplineRepository,
            IUnitOfWork unitOfWork)
        {
            _disciplineRepository = disciplineRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Discipline> GetDisciplineByIdAsync(int id, int tenantId)
        {
            return await _disciplineRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<Discipline>> GetAllDisciplinesAsync(int tenantId)
        {
            return await _disciplineRepository.GetAllAsync(tenantId);
        }

        public async Task<int> CreateDisciplineAsync(Discipline discipline, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure discipline belongs to the right tenant
                discipline.TenantID = tenantId;

                // Validate discipline data
                if (string.IsNullOrEmpty(discipline.DisciplineName))
                {
                    throw new ArgumentException("Discipline name is required");
                }

                // Check if discipline with same name already exists in this tenant
                var existingDiscipline = await _disciplineRepository.GetByNameAsync(discipline.DisciplineName, tenantId);
                if (existingDiscipline != null)
                {
                    throw new InvalidOperationException($"A discipline with name '{discipline.DisciplineName}' already exists");
                }

                // Set audit fields
                discipline.CreatedDate = DateTime.UtcNow;
                discipline.ModifiedDate = DateTime.UtcNow;
                discipline.IsActive = true;

                int id = await _disciplineRepository.InsertAsync(discipline);
                await _unitOfWork.CommitAsync();
                return id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateDisciplineAsync(Discipline discipline, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate discipline data
                if (string.IsNullOrEmpty(discipline.DisciplineName))
                {
                    throw new ArgumentException("Discipline name is required");
                }

                // Get existing discipline and verify it belongs to this tenant
                var existingDiscipline = await _disciplineRepository.GetByIdAsync(discipline.DisciplineID, tenantId);
                if (existingDiscipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {discipline.DisciplineID} not found");
                }

                // Ensure tenant ID cannot be changed
                discipline.TenantID = tenantId;

                // Check if another discipline with same name exists in this tenant
                var nameCheck = await _disciplineRepository.GetByNameAsync(discipline.DisciplineName, tenantId);
                if (nameCheck != null && nameCheck.DisciplineID != discipline.DisciplineID)
                {
                    throw new InvalidOperationException($"Another discipline with name '{discipline.DisciplineName}' already exists");
                }

                // Set audit fields
                discipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _disciplineRepository.UpdateAsync(discipline);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var discipline = await _disciplineRepository.GetByIdAsync(id, tenantId);
                if (discipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {id} not found");
                }

                // Perform soft delete by setting IsActive to false
                discipline.IsActive = false;
                discipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _disciplineRepository.UpdateAsync(discipline);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Discipline> GetDisciplineByNameAsync(string name, int tenantId)
        {
            return await _disciplineRepository.GetByNameAsync(name, tenantId);
        }

        public async Task<IEnumerable<Discipline>> GetActiveDisciplinesAsync(int tenantId)
        {
            return await _disciplineRepository.GetActiveAsync(tenantId);
        }

        public async Task<PagedResult<Discipline>> GetPagedDisciplinesAsync(int page, int pageSize, string searchTerm, int tenantId)
        {
            return await _disciplineRepository.GetPagedAsync(page, pageSize, searchTerm, tenantId);
        }

        public async Task<bool> ActivateDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var discipline = await _disciplineRepository.GetByIdAsync(id, tenantId);
                if (discipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {id} not found");
                }

                discipline.IsActive = true;
                discipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _disciplineRepository.UpdateAsync(discipline);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeactivateDisciplineAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify discipline belongs to this tenant
                var discipline = await _disciplineRepository.GetByIdAsync(id, tenantId);
                if (discipline == null)
                {
                    throw new InvalidOperationException($"Discipline with ID {id} not found");
                }

                discipline.IsActive = false;
                discipline.ModifiedDate = DateTime.UtcNow;

                bool result = await _disciplineRepository.UpdateAsync(discipline);

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
