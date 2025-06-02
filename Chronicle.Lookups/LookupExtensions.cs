using Chronicle.Caching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Lookups
{
    public static class LookupExtensions
    {
        /// <summary>
        /// Registers lookup services
        /// </summary>
        public static IServiceCollection AddChronicleLookupsServices(this IServiceCollection services)
        {
            // Register the ILookup service
            services.AddScoped<ILookup, Lookup>();

            // Subscribe to cache clear events
            services.AddScoped<ICacheClearEventHandler, LookupCacheClearHandler>();

            return services;
        }

        /// <summary>
        /// Handler to clear lookup caches when main cache is cleared
        /// </summary>
        public class LookupCacheClearHandler : ICacheClearEventHandler
        {
            private readonly ILookup _lookup;

            public LookupCacheClearHandler(ILookup lookup)
            {
                _lookup = lookup;
            }

            public void OnCacheCleared(string cacheKey)
            {
                // When companies cache is cleared, clear related lookup caches
                if (cacheKey == "CompaniesKey")
                {
                    // Make sure Lookup has a ClearLookupCaches method
                    if (_lookup is Lookup lookupImpl)
                    {
                        lookupImpl.ClearLookupCaches();
                    }
                }
            }
        }
    }
}
