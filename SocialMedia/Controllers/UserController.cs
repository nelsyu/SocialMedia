using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Services.Implements;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using System.Diagnostics;

namespace SocialMedia.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            // 若為登入狀態則跳轉到首頁
            if (_userService.IsLogin())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult Register(UserViewModel userVM)
        {
            List<string> result = _userService.ValidateRegister(userVM.Email, userVM.Username, userVM.Password, userVM.ConfirmPassword);

            // 如果 UserId 或 Email 已經存在，返回原始註冊頁面
            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                _userService.Register(userVM);

                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Login()
        {
            if (_userService.IsLogin())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserViewModel userVM)
        {
            List<string> result = _userService.ValidateLogin(userVM.Email, userVM.Password);

            // 如果 Email 或 Password 不存在，返回原始註冊頁面
            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);
                return View(userVM);
            }
            else
            {
                // 登入成功，導向到其他頁面
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            _userService.Logout();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult DeleteAccount(UserViewModel userVM)
        {
            _userService.DeleteAccount(userVM);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
