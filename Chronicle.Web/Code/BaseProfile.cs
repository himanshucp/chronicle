using AutoMapper;
using Chronicle.Caching;
using Microsoft.Extensions.Localization;

namespace Chronicle.Web.Code
{
    public abstract class BaseProfile : Profile
    {
        // Base class to all AutoMapper Profiles

        #region Lazy Dependency Injection

        // ** Lazy Injection pattern

        private static HttpContext HttpContext => ServiceLocator.Resolve<IHttpContextAccessor>().HttpContext!;

        //private ICurrentUser currentUser = null!;
        //private ICurrentTenant currentTenant = null!;
        //private IStringLocalizer<SharedResources> localizer = null!;

        //// Lifetime = Scoped
        protected ICache _cache => HttpContext.RequestServices.GetService<ICache>()!;
        //protected SaaSContext _db => HttpContext.RequestServices.GetService<SaaSContext>()!;

        //// Lifetime = Singleton
        //protected ICurrentUser _currentUser => currentUser ??= HttpContext.RequestServices.GetService<ICurrentUser>()!;
        //protected ICurrentTenant _currentTenant => currentTenant ??= HttpContext.RequestServices.GetService<ICurrentTenant>()!;
        //protected IStringLocalizer<SharedResources> _localizer => localizer ??= HttpContext.RequestServices.GetService<IStringLocalizer<SharedResources>>()!;

        #endregion
    }
}
