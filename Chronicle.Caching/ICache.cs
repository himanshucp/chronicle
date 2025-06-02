using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Caching
{
    public interface ICacheClearEventHandler
    {
        void OnCacheCleared(string cacheKey);
    }

    public interface ICache
    {
        Dictionary<int, Company> Companies { get; }

        Dictionary<int, Discipline> Disciplines { get; }

        void UpdateCompanyInCache(Company company);

        void UpdateDisciplineInCache(Discipline discipline);

        void ClearCompanies();
        void Clear();


        #region CompanyRoles 
        Dictionary<int, CompanyRole> CompanyRoles { get; }
        void UpdateCompanyRoleInCache(CompanyRole companyRole);

        void ClearCompanyRoles();

        #endregion

        #region CompanyRoles 
        Dictionary<int, HierarchyLevel> HierarchyLevels { get; }
        void UpdateHierarchyLevelInCache(HierarchyLevel hierarchyLevel);

        void ClearHierarchyLevels();

        #endregion 
    }
}
