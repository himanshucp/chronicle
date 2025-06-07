using Chronicle.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Chronicle.Entities;
using Dapper;

namespace Chronicle.Caching
{
    /// <summary>
    /// Memory-based implementation of ICache
    /// </summary>
    public class MemoryCache : ICache
    {
        private readonly IDapperContext _context;
        private readonly IMemoryCache _memoryCache;
        private static readonly object _locker = new object();

        // Cache keys
        private static readonly string CompaniesKey = nameof(CompaniesKey);
        private static readonly string DisciplineKey = nameof(DisciplineKey);
        private static readonly string CompanyRoleKey = nameof(CompanyRoleKey);
        private static readonly string HierarchyLevelKey = nameof(HierarchyLevelKey);
        private static readonly string ContractEmployeelRoleKey = nameof(ContractEmployeelRoleKey);




        // Keeps track of used keys
        private static readonly HashSet<string> UsedKeys = new HashSet<string>();

        // Expiration times
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(4);

        // Event to notify when cache is cleared
        public event Action<string> CacheCleared;

        public MemoryCache(IDapperContext context, IMemoryCache memoryCache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        #region Companies Cache

        /// <summary>
        /// Gets cached companies dictionary, loading from database if needed
        /// </summary>
        public Dictionary<int, Company> Companies
        {
            get
            {
                // Lazy load pattern 
                if (_memoryCache.Get(CompaniesKey) is not Dictionary<int, Company> dictionary)
                {
                    lock (_locker)
                    {
                        // Double-check inside lock
                        dictionary = _memoryCache.Get(CompaniesKey) as Dictionary<int, Company>;
                        if (dictionary == null)
                        {
                            dictionary = LoadCompaniesFromDatabase();
                            Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                        }
                    }
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Loads companies from database
        /// </summary>
        private Dictionary<int, Company> LoadCompaniesFromDatabase()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                    SELECT c.*, t.* 
                    FROM Companies c
                    LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                    ORDER BY c.Name";

                var companyDictionary = new Dictionary<int, Company>();
                var companies = connection.Query<Company, Tenant, Company>(
                    sql,
                    (company, tenant) =>
                    {
                        if (company != null)
                        {
                            company.Tenant = tenant;
                        }
                        return company;
                    },
                    splitOn: "TenantID");

                foreach (var company in companies)
                {
                    companyDictionary[company.CompanyID] = company;
                }

                return companyDictionary;
            }
        }

        /// <summary>
        /// Clears the companies cache
        /// </summary>
        public void ClearCompanies()
        {
            Clear(CompaniesKey);
            CacheCleared?.Invoke(CompaniesKey);
        }

        #endregion

        #region Dicpline



        /// <summary>
        /// Gets cached companies dictionary, loading from database if needed
        /// </summary>
        public Dictionary<int, Discipline> Disciplines  
        {
            get
            {
                // Lazy load pattern 
                if (_memoryCache.Get(DisciplineKey) is not Dictionary<int, Discipline> dictionary)
                {
                    lock (_locker)
                    {
                        // Double-check inside lock
                        dictionary = _memoryCache.Get(DisciplineKey) as Dictionary<int, Discipline>;
                        if (dictionary == null)
                        {
                            dictionary = LoadDisciplineFromDatabase();
                            Add(DisciplineKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                        }
                    }
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Loads companies from database
        /// </summary>
        private Dictionary<int, Discipline> LoadDisciplineFromDatabase()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                    SELECT c.*, t.* 
                    FROM Disciplines c
                    LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                    ORDER BY c.DisciplineName";

                var disciplineyDictionary = new Dictionary<int, Discipline>();
                var disciplines = connection.Query<Discipline, Tenant, Discipline>(
                    sql,
                    (discipline, tenant) =>
                    {
                        if (discipline != null)
                        {
                            discipline.TenantID = tenant.TenantID;
                        }
                        return discipline;
                    },
                    splitOn: "TenantID");

                foreach (var discipline in disciplines)
                {
                    disciplineyDictionary[discipline.DisciplineID] = discipline;
                }

                return disciplineyDictionary;
            }
        }

        /// <summary>
        /// Clears the companies cache
        /// </summary>
        public void ClearDiscipline()
        {
            Clear(DisciplineKey);
            CacheCleared?.Invoke(DisciplineKey);
        }

        #endregion

        #region Company Role 
        public Dictionary<int, CompanyRole> CompanyRoles
        {
            get
            {
                // Lazy load pattern 
                if (_memoryCache.Get(CompanyRoleKey) is not Dictionary<int, CompanyRole> dictionary)
                {
                    lock (_locker)
                    {
                        // Double-check inside lock
                        dictionary = _memoryCache.Get(CompanyRoleKey) as Dictionary<int, CompanyRole>;
                        if (dictionary == null)
                        {
                            dictionary = LoadCompanyRoleFromDatabase();
                            Add(CompanyRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                        }
                    }
                }

                return dictionary;
            }
        }

     

        /// <summary>
        /// Loads companies from database
        /// </summary>
        private Dictionary<int, CompanyRole> LoadCompanyRoleFromDatabase()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                    SELECT c.*, t.* 
                                 FROM CompanyRoles c
                                 LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                                 ORDER BY c.RoleName";

                var companyRoleDictionary = new Dictionary<int, CompanyRole>();
                var companyRoles = connection.Query<CompanyRole, Tenant, CompanyRole>(
                    sql,
                    (companyRole, tenant) =>
                    {
                        if (companyRole != null)
                        {
                            companyRole.Tenant = tenant;
                        }
                        return companyRole;
                    },
                    splitOn: "TenantID");

                foreach (var companyRole in companyRoles)
                {
                    companyRoleDictionary[companyRole.CompanyRoleID] = companyRole;
                }

                return companyRoleDictionary;
            }
        }

        /// <summary>
        /// Clears the companies cache
        /// </summary>
        public void ClearCompanyRoles()
        {
            Clear(CompanyRoleKey);
            CacheCleared?.Invoke(CompanyRoleKey);
        }

        #endregion



        #region HierarchyLevels
        public Dictionary<int, HierarchyLevel> HierarchyLevels
        {
            get
            {
                // Lazy load pattern 
                if (_memoryCache.Get(HierarchyLevelKey) is not Dictionary<int, HierarchyLevel> dictionary)
                {
                    lock (_locker)
                    {
                        // Double-check inside lock
                        dictionary = _memoryCache.Get(HierarchyLevelKey) as Dictionary<int, HierarchyLevel>;
                        if (dictionary == null)
                        {
                            dictionary = LoadHierarchyLevelFromDatabase();
                            Add(HierarchyLevelKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                        }
                    }
                }

                return dictionary;
            }
        }



        /// <summary>
        /// Loads companies from database
        /// </summary>
        private Dictionary<int, HierarchyLevel> LoadHierarchyLevelFromDatabase()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                    SELECT c.*, t.* 
                                 FROM HierarchyLevels c
                                 LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                                 ORDER BY c.LevelName";

                var hierarchyLevelDictionary = new Dictionary<int, HierarchyLevel>();
                var hierarchyLevels = connection.Query<HierarchyLevel, Tenant, HierarchyLevel>(
                    sql,
                    (hierarchyLevel, tenant) =>
                    {
                        if (hierarchyLevel != null)
                        {
                            hierarchyLevel.Tenant = tenant;
                        }
                        return hierarchyLevel;
                    },
                    splitOn: "TenantID");

                foreach (var hierarchyLevel in hierarchyLevels)
                {
                    hierarchyLevelDictionary[hierarchyLevel.LevelID] = hierarchyLevel;
                }

                return hierarchyLevelDictionary;
            }
        }

        private async Task<Dictionary<int, HierarchyLevel>> LoadHierarchyLevelFromDatabaseAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                    SELECT c.*, t.* 
                                 FROM HierarchyLevels c
                                 LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                                 ORDER BY c.LevelName";

                var hierarchyLevelDictionary = new Dictionary<int, HierarchyLevel>();
                var hierarchyLevels = connection.Query<HierarchyLevel, Tenant, HierarchyLevel>(
                    sql,
                    (hierarchyLevel, tenant) =>
                    {
                        if (hierarchyLevel != null)
                        {
                            hierarchyLevel.Tenant = tenant;
                        }
                        return hierarchyLevel;
                    },
                    splitOn: "TenantID");

                foreach (var hierarchyLevel in hierarchyLevels)
                {
                    hierarchyLevelDictionary[hierarchyLevel.LevelID] = hierarchyLevel;
                }

                return hierarchyLevelDictionary;
            }
        }

        public async Task EnsureHierarchyLeveLoadedAsync()
        {
            if (_memoryCache.Get(HierarchyLevelKey) is not Dictionary<int, HierarchyLevel>)
            {
                await LoadHierarchyLevelFromDatabaseAsync();
            }
        }

        public void UpdateHierarchyLevelInCache(HierarchyLevel hierarchyLevel)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(HierarchyLevelKey) is Dictionary<int, HierarchyLevel> dictionary)
                {
                    dictionary[hierarchyLevel.LevelID] = hierarchyLevel;

                    // Reset expiration time
                    _memoryCache.Remove(HierarchyLevelKey);
                    Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                    // Notify listeners of update
                    CacheCleared?.Invoke(HierarchyLevelKey);
                }
            }
        }

        /// <summary>
        /// Refreshes a specific cache entry
        /// </summary>
        public async Task RefreshHierarchyLevelAsync()
        {
            var hierarchyLevels = await LoadHierarchyLevelFromDatabaseAsync();

            lock (_locker)
            {
                // Replace existing cache with fresh data
                _memoryCache.Remove(HierarchyLevelKey);
                Add(HierarchyLevelKey, hierarchyLevels, DateTime.UtcNow.Add(DefaultExpiration));
            }

            // Notify listeners
            CacheCleared?.Invoke(HierarchyLevelKey);
        }


        /// <summary>
        /// Removes a company from the cache
        /// </summary>
        public void RemoveHierarchyLevelFromCache(int levelId)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(HierarchyLevelKey) is Dictionary<int, HierarchyLevel> dictionary)
                {
                    if (dictionary.ContainsKey(levelId))
                    {
                        dictionary.Remove(levelId);

                        // Reset expiration time
                        _memoryCache.Remove(HierarchyLevelKey);
                        Add(HierarchyLevelKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                        // Notify listeners of update
                        CacheCleared?.Invoke(HierarchyLevelKey);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the companies cache
        /// </summary>
        public void ClearHierarchyLevels()
        {
            Clear(HierarchyLevelKey);
            CacheCleared?.Invoke(HierarchyLevelKey);
        }

        #endregion


        #region ContractEmployeeRole

        public Dictionary<int, ContractEmployeeRole> ContractEmployeeRoles
        {
            get
            {
                // Lazy load pattern 
                if (_memoryCache.Get(ContractEmployeelRoleKey) is not Dictionary<int, ContractEmployeeRole> dictionary)
                {
                    lock (_locker)
                    {
                        // Double-check inside lock
                        dictionary = _memoryCache.Get(ContractEmployeelRoleKey) as Dictionary<int, ContractEmployeeRole>;
                        if (dictionary == null)
                        {
                            dictionary = LoadContractEmployeeRoleFromDatabase();
                            Add(CompanyRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                        }
                    }
                }

                return dictionary;
            }
        }



        /// <summary>
        /// Loads companies from database
        /// </summary>
        private Dictionary<int, ContractEmployeeRole> LoadContractEmployeeRoleFromDatabase()
        {
            using (var connection = _context.CreateConnection())
            {
                // Load companies with appropriate ordering
                const string sql = @"
                     SELECT c.*, t.* 
                          FROM ContractEmployeeRoles c
                          LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                          ORDER BY c.RoleName";

                var contracrEmployeeRoleDictionary = new Dictionary<int, ContractEmployeeRole>();
                var contractEmployRoles = connection.Query<ContractEmployeeRole, Tenant, ContractEmployeeRole>(
                    sql,
                    (contractEmployeeRole, tenant) =>
                    {
                        if (contractEmployeeRole != null)
                        {
                            contractEmployeeRole.Tenant = tenant;
                        }
                        return contractEmployeeRole;
                    },
                    splitOn: "TenantID");

                foreach (var contractEmployeeRole in contractEmployRoles)
                {
                    contracrEmployeeRoleDictionary[contractEmployeeRole.ContractRoleID] = contractEmployeeRole;
                }

                return contracrEmployeeRoleDictionary;
            }
        }

        private async Task<Dictionary<int, ContractEmployeeRole>> LoadContractEmployeeRoleFromDatabaseAsync()
        {
            Dictionary<int, ContractEmployeeRole> dictionary = null;

            // Use a semaphore or similar to prevent multiple concurrent loads
            lock (_locker)
            {
                // Double-check inside lock
                dictionary = _memoryCache.Get(ContractEmployeelRoleKey) as Dictionary<int, ContractEmployeeRole>;
                if (dictionary != null)
                {
                    return dictionary;
                }
            }

            using (var connection = await _context.CreateConnectionAsync())
            {
                const string sql = @"
                     SELECT c.*, t.* 
                          FROM ContractEmployeeRoles c
                          LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                          ORDER BY c.RoleName";

                var contracrEmployeeRoleDictionary = new Dictionary<int, ContractEmployeeRole>();
                var contractEmployRoles = connection.Query<ContractEmployeeRole, Tenant, ContractEmployeeRole>(
                    sql,
                    (contractEmployeeRole, tenant) =>
                    {
                        if (contractEmployeeRole != null)
                        {
                            contractEmployeeRole.Tenant = tenant;
                        }
                        return contractEmployeeRole;
                    },
                    splitOn: "TenantID");

                foreach (var contractEmployeeRole in contractEmployRoles)
                {
                    contracrEmployeeRoleDictionary[contractEmployeeRole.ContractRoleID] = contractEmployeeRole;
                }

                dictionary = contracrEmployeeRoleDictionary;

                // Add to cache
                lock (_locker)
                {
                    Add(CompanyRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                }

                return dictionary;
            }
        }


        public async Task RefreshContractEmployeeRolesAsync()
        {
            var contractEmployeeRole = await LoadContractEmployeeRoleFromDatabaseAsync();

            lock (_locker)
            {
                // Replace existing cache with fresh data
                _memoryCache.Remove(ContractEmployeelRoleKey);
                Add(ContractEmployeelRoleKey, contractEmployeeRole, DateTime.UtcNow.Add(DefaultExpiration));
            }

            // Notify listeners
            CacheCleared?.Invoke(CompanyRoleKey);
        }

        /// <summary>
        /// Updates a specific company in the cache
        /// </summary>
        public void UpdateContractEmployeeRoleInCache(ContractEmployeeRole  contractEmployeeRole)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(ContractEmployeelRoleKey) is Dictionary<int, ContractEmployeeRole> dictionary)
                {
                    dictionary[contractEmployeeRole.ContractRoleID] = contractEmployeeRole;

                    // Reset expiration time
                    _memoryCache.Remove(CompanyRoleKey);
                    Add(ContractEmployeelRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                    // Notify listeners of update
                    CacheCleared?.Invoke(ContractEmployeelRoleKey);
                }
            }
        }

        /// <summary>
        /// Removes a company from the cache
        /// </summary>
        public void RemoveContractEmployeeRoleFromCache(int contractRoleId)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(CompanyRoleKey) is Dictionary<int, ContractEmployeeRole> dictionary)
                {
                    if (dictionary.ContainsKey(contractRoleId))
                    {
                        dictionary.Remove(contractRoleId);

                        // Reset expiration time
                        _memoryCache.Remove(CompanyRoleKey);
                        Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                        // Notify listeners of update
                        CacheCleared?.Invoke(CompanyRoleKey);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the companies cache
        /// </summary>
        public void ClearContractEmployeeRoles()
        {
            Clear(ContractEmployeelRoleKey);
            CacheCleared?.Invoke(ContractEmployeelRoleKey);
        }

        #endregion

  

        #region Cache Helpers

        /// <summary>
        /// Clears a single cache entry
        /// </summary>
        private void Clear(string key)
        {
            lock (_locker)
            {
                _memoryCache.Remove(key);
                UsedKeys.Remove(key);
            }
        }

        /// <summary>
        /// Clears the entire cache
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                foreach (var usedKey in UsedKeys.ToList())
                {
                    _memoryCache.Remove(usedKey);
                    // Trigger events for each cleared key
                    CacheCleared?.Invoke(usedKey);
                }

                UsedKeys.Clear();
            }
        }

        /// <summary>
        /// Adds an item to the cache
        /// </summary>
        private void Add(string key, object value, DateTimeOffset expiration)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration)
                .RegisterPostEvictionCallback((removedKey, value, reason, state) =>
                {
                    // Remove from used keys when evicted
                    if (reason != EvictionReason.Replaced)
                    {
                        lock (_locker)
                        {
                            UsedKeys.Remove(removedKey.ToString());
                        }
                    }
                });

            _memoryCache.Set(key, value, options);

            lock (_locker)
            {
                UsedKeys.Add(key);
            }
        }

        #endregion

        #region Async Support

        /// <summary>
        /// Asynchronously ensures the companies cache is loaded
        /// </summary>
        public async Task EnsureCompaniesLoadedAsync()
        {
            if (_memoryCache.Get(CompaniesKey) is not Dictionary<int, Company>)
            {
                await LoadCompaniesFromDatabaseAsync();
            }
        }

        /// <summary>
        /// Asynchronously loads companies from database
        /// </summary>
        private async Task<Dictionary<int, Company>> LoadCompaniesFromDatabaseAsync()
        {
            Dictionary<int, Company> dictionary = null;

            // Use a semaphore or similar to prevent multiple concurrent loads
            lock (_locker)
            {
                // Double-check inside lock
                dictionary = _memoryCache.Get(CompaniesKey) as Dictionary<int, Company>;
                if (dictionary != null)
                {
                    return dictionary;
                }
            }

            using (var connection = await _context.CreateConnectionAsync())
            {
                const string sql = @"
                    SELECT c.*, t.* 
                    FROM Companies c
                    LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                    ORDER BY c.Name";

                var companyDictionary = new Dictionary<int, Company>();
                var companies = await connection.QueryAsync<Company, Tenant, Company>(
                    sql,
                    (company, tenant) =>
                    {
                        if (company != null)
                        {
                            company.Tenant = tenant;
                        }
                        return company;
                    },
                    splitOn: "TenantID");

                foreach (var company in companies)
                {
                    companyDictionary[company.CompanyID] = company;
                }

                dictionary = companyDictionary;

                // Add to cache
                lock (_locker)
                {
                    Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                }

                return dictionary;
            }
        }


        /// <summary>
        /// Asynchronously ensures the companies cache is loaded
        /// </summary>
        public async Task EnsureCompanuRoleLoadedAsync()
        {
            if (_memoryCache.Get(CompanyRoleKey) is not Dictionary<int, CompanyRole>)
            {
                await LoadCompanyRoleFromDatabaseAsync();
            }
        }

        /// <summary>
        /// Asynchronously loads companies from database
        /// </summary>
        private async Task<Dictionary<int, CompanyRole>> LoadCompanyRoleFromDatabaseAsync()
        {
            Dictionary<int, CompanyRole> dictionary = null;

            // Use a semaphore or similar to prevent multiple concurrent loads
            lock (_locker)
            {
                // Double-check inside lock
                dictionary = _memoryCache.Get(CompanyRoleKey) as Dictionary<int, CompanyRole>;
                if (dictionary != null)
                {
                    return dictionary;
                }
            }

            using (var connection = await _context.CreateConnectionAsync())
            {
                const string sql = @"
                    SELECT c.*, t.* 
                         FROM CompanyRoles c
                         LEFT JOIN Tenants t ON c.TenantID = t.TenantID
                         ORDER BY c.RoleName";

                var companyRoleDictionary = new Dictionary<int, CompanyRole>();
                var companyRoles = await connection.QueryAsync<CompanyRole, Tenant, CompanyRole>(
                    sql,
                    (companyRole, tenant) =>
                    {
                        if (companyRole != null)
                        {
                            companyRole.Tenant = tenant;
                        }
                        return companyRole;
                    },
                    splitOn: "TenantID");

                foreach (var companyRole in companyRoles)
                {
                    companyRoleDictionary[companyRole.CompanyRoleID] = companyRole;
                }

                dictionary = companyRoleDictionary;

                // Add to cache
                lock (_locker)
                {
                    Add(CompanyRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));
                }

                return dictionary;
            }
        }
        #endregion

        #region Multi-Tenant Support

        /// <summary>
        /// Gets companies for a specific tenant
        /// </summary>
        public Dictionary<int, Company> GetCompaniesByTenant(int tenantId)
        {
            return Companies
                .Where(kvp => kvp.Value.TenantID == tenantId)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        #endregion

        #region Extended Cache Management

        /// <summary>
        /// Refreshes a specific cache entry
        /// </summary>
        public async Task RefreshCompaniesAsync()
        {
            var companies = await LoadCompaniesFromDatabaseAsync();

            lock (_locker)
            {
                // Replace existing cache with fresh data
                _memoryCache.Remove(CompaniesKey);
                Add(CompaniesKey, companies, DateTime.UtcNow.Add(DefaultExpiration));
            }

            // Notify listeners
            CacheCleared?.Invoke(CompaniesKey);
        }

        /// <summary>
        /// Updates a specific company in the cache
        /// </summary>
        public void UpdateCompanyInCache(Company company)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(CompaniesKey) is Dictionary<int, Company> dictionary)
                {
                    dictionary[company.CompanyID] = company;

                    // Reset expiration time
                    _memoryCache.Remove(CompaniesKey);
                    Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                    // Notify listeners of update
                    CacheCleared?.Invoke(CompaniesKey);
                }
            }
        }

        /// <summary>
        /// Removes a company from the cache
        /// </summary>
        public void RemoveCompanyFromCache(int companyId)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(CompaniesKey) is Dictionary<int, Company> dictionary)
                {
                    if (dictionary.ContainsKey(companyId))
                    {
                        dictionary.Remove(companyId);

                        // Reset expiration time
                        _memoryCache.Remove(CompaniesKey);
                        Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                        // Notify listeners of update
                        CacheCleared?.Invoke(CompaniesKey);
                    }
                }
            }
        }

        public void UpdateDisciplineInCache(Discipline discipline)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(DisciplineKey) is Dictionary<int, Discipline> dictionary)
                {
                    dictionary[discipline.DisciplineID] = discipline;

                    // Reset expiration time
                    _memoryCache.Remove(DisciplineKey);
                    Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                    // Notify listeners of update
                    CacheCleared?.Invoke(DisciplineKey);
                }
            }
        }

        /// <summary>
        /// Removes a company from the cache
        /// </summary>
        public void RemoveDisciplineFromCache(int disciplineID)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(DisciplineKey) is Dictionary<int, Company> dictionary)
                {
                    if (dictionary.ContainsKey(disciplineID))
                    {
                        dictionary.Remove(disciplineID);

                        // Reset expiration time
                        _memoryCache.Remove(DisciplineKey);
                        Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                        // Notify listeners of update
                        CacheCleared?.Invoke(DisciplineKey);
                    }
                }
            }
        }



        #region Company Role

        public async Task RefreshCompanyRolesAsync()
        {
            var companies = await LoadCompanyRoleFromDatabaseAsync();

            lock (_locker)
            {
                // Replace existing cache with fresh data
                _memoryCache.Remove(CompanyRoleKey);
                Add(CompanyRoleKey, companies, DateTime.UtcNow.Add(DefaultExpiration));
            }

            // Notify listeners
            CacheCleared?.Invoke(CompanyRoleKey);
        }

        /// <summary>
        /// Updates a specific company in the cache
        /// </summary>
        public void UpdateCompanyRoleInCache(CompanyRole companyRole)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(CompanyRoleKey) is Dictionary<int, CompanyRole> dictionary)
                {
                    dictionary[companyRole.CompanyRoleID] = companyRole;

                    // Reset expiration time
                    _memoryCache.Remove(CompanyRoleKey);
                    Add(CompanyRoleKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                    // Notify listeners of update
                    CacheCleared?.Invoke(CompanyRoleKey);
                }
            }
        }

        /// <summary>
        /// Removes a company from the cache
        /// </summary>
        public void RemoveCompanyRoleFromCache(int companyRoleId)
        {
            lock (_locker)
            {
                if (_memoryCache.Get(CompanyRoleKey) is Dictionary<int, CompanyRole> dictionary)
                {
                    if (dictionary.ContainsKey(companyRoleId))
                    {
                        dictionary.Remove(companyRoleId);

                        // Reset expiration time
                        _memoryCache.Remove(CompanyRoleKey);
                        Add(CompaniesKey, dictionary, DateTime.UtcNow.Add(DefaultExpiration));

                        // Notify listeners of update
                        CacheCleared?.Invoke(CompanyRoleKey);
                    }
                }
            }
        }

       

        #endregion

        #endregion
    }
}
