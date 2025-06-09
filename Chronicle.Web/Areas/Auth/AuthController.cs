using Chronicle.Entities;
using Chronicle.Services;
using Chronicle.Web.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Chronicle.Services.UserServices;

namespace Chronicle.Web.Areas.Auth
{
    [Area("Auth")]
    public class AuthController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(
            IUserService userService,
            IRoleService roleService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet("/Auth/Login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost("/Auth/Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model, string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            //var result = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);

            //if (!result.Success)
            //{
            //    ModelState.AddModelError(string.Empty, result.Message);
            //    Failure = result.Message;
            //    return View(model);
            //}

            //// Get user and roles
            //var userResult = await _userService.GetUserByUsernameAsync(model.Username);

            //if (!userResult.Success)
            //{
            //    ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
            //    Failure = "Login failed. Please try again";
            //    return View(model);
            //}

            //Entities.User user = userResult.Data;

            //var rolesResult = await _roleService.GetRolesByUserAsync(user.UserId);

            //// Create claims
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            //    new Claim(ClaimTypes.Name, user.Username),
            //    new Claim(ClaimTypes.Email, user.Email)
            //};

            //// Add role claims
            //if (rolesResult.Success)
            //{
            //    foreach (var role in rolesResult.Data)
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, role.Name));
            //    }
            //}

            //var claimsIdentity = new ClaimsIdentity(
            //    claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //var authProperties = new AuthenticationProperties
            //{
            //    IsPersistent = model.RememberMe,
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1)
            //};

            //await HttpContext.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    new ClaimsPrincipal(claimsIdentity),
            //    authProperties);

            //_logger.LogInformation("User {Username} logged in at {Time}", user.Username, DateTime.UtcNow);

            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }



    }
}
