using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtpNet;
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
            List<string> result = _userService.ValidateRegister(userVM);

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

            byte[] captchaImage = _userService.GenerateCaptchaImage(out string captchaCode);
            ViewBag.CaptchaImage = captchaImage;
            TempData["CaptchaCode"] = captchaCode;

            return View();
        }

        [HttpGet]
        public IActionResult RefreshCaptcha()
        {
            if (_userService.IsLogin())
                return RedirectToAction("Index", "Home");

            byte[] captchaImage = _userService.GenerateCaptchaImage(out string captchaCode);
            TempData["CaptchaCode"] = captchaCode;

            return Json(new { image = Convert.ToBase64String(captchaImage), code = captchaCode });
        }

        [HttpPost]
        public IActionResult Login(UserViewModel userVM)
        {
            List<string> result = _userService.ValidateLogin(userVM, TempData["CaptchaCode"] ?? "");

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                // 生成 captcha 圖片
                byte[] captchaImage = _userService.GenerateCaptchaImage(out string captchaCode);
                ViewBag.CaptchaImage = captchaImage;
                TempData["CaptchaCode"] = captchaCode;

                return View(userVM);
            }
            else
            {
                HttpContext.Session.SetString("UserVMEmail", userVM.Email);

                return RedirectToAction("ValidateQRCodeOTP", "User");
            }
        }

        [HttpPost]
        public IActionResult SaveQRCodeOTP([FromForm] string QRCodeOTPSK)
        {
             _userService.SaveQRCodeOTP(QRCodeOTPSK);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ValidateQRCodeOTP()
        {
            string userVMEmail = HttpContext.Session.GetString("UserVMEmail") ?? "";

            if (string.IsNullOrEmpty(userVMEmail))
                return RedirectToAction("Login", "User");

            if (_userService.IsQRCodeOTPSecretKey(userVMEmail) == null)
            {
                _userService.LoginSuccessful(userVMEmail);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ValidateQRCodeOTP(UserViewModel userVM)
        {
            string userVMEmail = HttpContext.Session.GetString("UserVMEmail") ?? "";

            if (string.IsNullOrEmpty(userVMEmail))
                return RedirectToAction("Login", "User");

            List<string> result = _userService.VerifyQRCodeOTP(userVMEmail, userVM.ConfirmQRCodeOTP);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                _userService.LoginSuccessful(userVMEmail);

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
