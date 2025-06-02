using AutoMapper;
using Chronicle.Caching;
using Chronicle.Lookups;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web
{
    public class BaseController : Controller
    {

        private ICache cache = null!;
        private ILookup lookup = null!;
        private IMapper mapper = null!;


        protected ICache _cache => cache ??= HttpContext.RequestServices.GetService<ICache>()!;

        protected ILookup _lookup => lookup ??= HttpContext.RequestServices.GetService<ILookup>()!;
        protected IMapper _mapper => mapper ??= HttpContext.RequestServices.GetService<IMapper>()!;


        #region Meta

        public string? Title { set => ViewBag.Title = value; }
        public string? Keywords { set => ViewBag.Keywords = value; }
        public string? Description { set => ViewBag.Description = value; }

        #endregion

        #region Alerts

        // Success and Failure contain alert messages that are available even following a redirect.

        public string? Success { set => TempData["Success"] = value; get => TempData["Success"]?.ToString(); }
        public string? Failure { set => TempData["Failure"] = value; get => TempData["Failure"]?.ToString(); }

        #endregion

        #region SQL helpers

        //protected string AndTenant(string where) => where.ANDTenant(_currentTenant.Id);
        //protected string SingleWhereId => $"Id = @0 AND TenantId = {_currentTenant.Id}";

        #endregion
    }
}
