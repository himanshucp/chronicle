using Chronicle.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronicle.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chronicle.Lookups
{
    public class Lookup : ILookup
    {
        private readonly ICache _cache;
        private readonly IMemoryCache _lookupCache;
        private static readonly TimeSpan _lookupCacheDuration = TimeSpan.FromMinutes(10);

        // Cache keys - important to avoid collisions
        private const string CompanyItemsKey = "Lookup_CompanyItems";
        private const string CompanyLocationItemsKey = "Lookup_CompanyLocationItems";
        private const string ActiveCompanyItemsKey = "Lookup_ActiveCompanyItems";
        private const string DisciplineItemsKey = "Lookup_Discipline";
        private const string CompanyRoleItemsKey = "Lookup_CompanyRole";
        private const string HierarchyLevelItemsKey = "Lookup_HierarchyLevel";
        private const string ContractEmployeeRoleItemsKey = "Lookup_ContractEmployeeRole";

        public Lookup(ICache cache, IMemoryCache memoryCache)
        {
            _cache = cache;
            _lookupCache = memoryCache;
        }

        #region Company Lookups

        public List<SelectListItem> GetCompanyItems(bool includeEmpty = true, string emptyText = "-- Select Company --")
        {
            string cacheKey = $"{CompanyItemsKey}_{includeEmpty}_{emptyText}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = emptyText, Selected = true });
                }

                // Get from main cache
                var companies = _cache.Companies.Values
                                      .OrderBy(c => c.Name)  // Make sure this matches your property name
                                      .ToList();

                foreach (var company in companies)
                {
                    list.Add(new SelectListItem
                    {
                        Value = company.CompanyID.ToString(),  // Make sure this matches your property name
                        Text = company.Name  // Make sure this matches your property name
                    });
                }

                return list;
            });
        }
        public List<SelectListItem> GetCompanyItemsByLocation(string location = null, bool includeEmpty = true)
        {
            string cacheKey = $"Lookup_CompanyItemsByLocation_{location}_{includeEmpty}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = "-- Select Company --", Selected = true });
                }

                var companies = _cache.Companies.Values.ToList();

                // Filter by location if specified
                if (!string.IsNullOrEmpty(location))
                {
                    companies = companies.Where(c => c.Location == location).ToList();
                }

                // Group and order for display
                var orderedCompanies = companies
                    .OrderBy(c => c.Location)
                    .ThenBy(c => c.Name);

                foreach (var company in orderedCompanies)
                {
                    string displayText = string.IsNullOrEmpty(company.Location)
                        ? company.Name
                        : $"{company.Name} ({company.Location})";

                    list.Add(new SelectListItem
                    {
                        Value = company.CompanyID.ToString(),
                        Text = displayText
                    });
                }

                return list;
            });
        }

        public List<SelectListItem> GetCompanyLocationItems(bool includeEmpty = true)
        {
            string cacheKey = $"{CompanyLocationItemsKey}_{includeEmpty}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = "-- Select Location --", Selected = true });
                }

                var locations = _cache.Companies.Values
                    .Where(c => !string.IsNullOrEmpty(c.Location))
                    .Select(c => c.Location)
                    .Distinct()
                    .OrderBy(l => l);

                foreach (var location in locations)
                {
                    list.Add(new SelectListItem
                    {
                        Value = location,
                        Text = location
                    });
                }

                return list;
            });
        }

        public List<SelectListItem> GetActiveCompanyItems(bool includeEmpty = true)
        {
            string cacheKey = $"{ActiveCompanyItemsKey}_{includeEmpty}";
            var list = new List<SelectListItem>() { new(value: "", text: "-- Select --", selected: true) };


            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = "-- Select Active Company --", Selected = true });
                }

                var activeCompanies = _cache.Companies.Values
                    .Where(c => c.IsActive == true)
                    .OrderBy(c => c.Name);

                foreach (var company in activeCompanies)
                {
                    list.Add(new SelectListItem
                    {
                        Value = company.CompanyID.ToString(),
                        Text = company.Name
                    });
                }

                return list;
            });
        }


        public List<SelectListItem> GetDisciplineItems(bool includeEmpty = true, string emptyText = "-- Select Discipline --")
        {
            string cacheKey = $"{DisciplineItemsKey}_{includeEmpty}_{emptyText}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = emptyText, Selected = true });
                }

                // Get from main cache
                var disciplines = _cache.Disciplines.Values
                                      .OrderBy(c => c.DisciplineName)  // Make sure this matches your property name
                                      .ToList();

                foreach (var discipline in disciplines)
                {
                    list.Add(new SelectListItem
                    {
                        Value = discipline.DisciplineID.ToString(),  // Make sure this matches your property name
                        Text = discipline.DisciplineName  // Make sure this matches your property name
                    });
                }

                return list;
            });
        }



        public List<SelectListItem> GetCompanyRoleItems(bool includeEmpty = true, string emptyText = "-- Select Company Role --")
        {
            string cacheKey = $"{CompanyRoleItemsKey}_{includeEmpty}_{emptyText}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = emptyText, Selected = true });
                }

                // Get from main cache
                var disciplines = _cache.CompanyRoles.Values
                                      .OrderBy(c => c.RoleName)  // Make sure this matches your property name
                                      .ToList();

                foreach (var discipline in disciplines)
                {
                    list.Add(new SelectListItem
                    {
                        Value = discipline.CompanyRoleID.ToString(),  // Make sure this matches your property name
                        Text = discipline.RoleName  // Make sure this matches your property name
                    });
                }

                return list;
            });
        }


        public List<SelectListItem> GetHierarchyLevelsItems(bool includeEmpty = true, string emptyText = "-- Select Hierarchy Levels --")
        {
            string cacheKey = $"{HierarchyLevelItemsKey}_{includeEmpty}_{emptyText}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = emptyText, Selected = true });
                }

                // Get from main cache
                var hierarchyLevels = _cache.HierarchyLevels.Values
                                      .OrderBy(c => c.LevelName)  // Make sure this matches your property name
                                      .ToList();

                foreach (var hierarchyLevel in hierarchyLevels)
                {
                    list.Add(new SelectListItem
                    {
                        Value = hierarchyLevel.LevelID.ToString(),  // Make sure this matches your property name
                        Text = hierarchyLevel.LevelName  // Make sure this matches your property name
                    });
                }

                return list;
            });
        }

        public List<SelectListItem> GetContractEmployeeRoleItems(bool includeEmpty = true, string emptyText = "-- Select Individual Role --")
        {
            string cacheKey = $"{ContractEmployeeRoleItemsKey}_{includeEmpty}_{emptyText}";

            return _lookupCache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = _lookupCacheDuration;

                var list = new List<SelectListItem>();

                if (includeEmpty)
                {
                    list.Add(new SelectListItem { Value = "", Text = emptyText, Selected = true });
                }

                // Get from main cache
                var hierarchyLevels = _cache.ContractEmployeeRoles.Values
                                      .OrderBy(c => c.RoleName)  // Make sure this matches your property name
                                      .ToList();

                foreach (var hierarchyLevel in hierarchyLevels)
                {
                    list.Add(new SelectListItem
                    {
                        Value = hierarchyLevel.ContractRoleID.ToString(),  // Make sure this matches your property name
                        Text = hierarchyLevel.RoleName  // Make sure this matches your property name
                    });
                }

                return list;
            });
        }

        public List<SelectListItem> StatusItems =>
              new()
              {
                   new SelectListItem { Value = "", Text = "All Status" , Selected = true },
                   new SelectListItem { Value = "Active", Text = "Active"},
                   new SelectListItem { Value = "Pending", Text = "Pending"},
                   new SelectListItem { Value = "Expired", Text = "Expired"},
                   new SelectListItem { Value = "Draft", Text = "Draft"},

              };

        #endregion

        // Additional lookup methods can be added here

        /// <summary>
        /// Clears all lookup caches - should be called when underlying data changes
        /// </summary>
        public void ClearLookupCaches()
        {
            // Clear all lookup caches when data changes
            var cacheKeys = new[]
            {
                CompanyItemsKey,
                CompanyLocationItemsKey,
                ActiveCompanyItemsKey
                // Add other keys as needed
            };

            foreach (var keyPrefix in cacheKeys)
            {
                // Use reflection or a cache entry scanning approach to clear
                // all entries that start with the prefix
            }
        }
    }
}
