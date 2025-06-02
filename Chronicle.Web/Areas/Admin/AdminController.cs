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
    }
}
