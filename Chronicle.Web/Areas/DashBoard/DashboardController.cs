using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.DashBoard
{

    [Area("DashBoard")]
    public class DashboardController : BaseController 
    {

        [HttpGet("WorkFlowDashBoard")]
        public IActionResult WorkflowDashboard()
        {
            return View();
        }
    }
}
