using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.Admin
{
    [Area("Admin")]
    public class AdminController : BaseController
    {

        [HttpGet("Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [HttpGet("ClearCache")]
        public IActionResult ClearCache()
        {
            _cache.Clear();

            Success = "Caches have been cleared";

            return LocalRedirect("/admin");
        }
    }
}
