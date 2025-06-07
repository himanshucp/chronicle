using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.SiteInspection
{
    [Area("SiteInspection")]
    public class SiteInspectionController : BaseController
    {
        [HttpGet("/Inspection")]
        public IActionResult InspectionList()
        {
            return View();
        }

        [HttpGet("/Inspection/Create")]
        public IActionResult Create()
        {
            return View();
        }
    }
}
