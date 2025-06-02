using Chronicle.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Areas.Users
{
    [Area("Users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Users(
               string searchTerm = "",
               int page = 1,
               int pageSize = 10)
        {
            //var result = await _userService.GetUsersAsync(page, pageSize, searchTerm);

            //if (!result.Success)
            //{
            //    TempData["ErrorMessage"] = result.Message;
            //    return View(new UserListViewModel());
            //}

            //var model = new UserListViewModel
            //{
            //    Users = result.Data.Items,
            //    TotalCount = result.Data.TotalCount,
            //    CurrentPage = page,
            //    PageSize = pageSize,
            //    SearchTerm = searchTerm
            //};

            return View();
        }
    }
}
