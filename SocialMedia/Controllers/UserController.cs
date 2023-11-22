using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using SocialMedia.Models;
using System.Diagnostics;
using Library.Extensions;

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

        public async Task<IActionResult> Register()
        {
            // 若為登入狀態則跳轉到首頁
            if (await _userService.IsLoginAsync())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userVM)
        {
            List<string> result = await _userService.ValidateRegisterAsync(userVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                await _userService.RegisterAsync(userVM);

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Login()
        {
            if (await _userService.IsLoginAsync())
                return RedirectToAction("Index", "Home");

            (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();

            UserViewModel userVM = new()
            {
                CaptchaImage = captchaImage
            };
            HttpContext.Session.SetString(SessionKeys.CaptchaCode, captchaCode);

            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshCaptcha()
        {
            (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();
            HttpContext.Session.SetString(SessionKeys.CaptchaCode, captchaCode);

            return Json(new { image = Convert.ToBase64String(captchaImage) });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            List<string> result = await _userService.ValidateLoginAsync(userVM, HttpContext.Session.GetString(SessionKeys.CaptchaCode) ?? "");

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                // 生成 captcha 圖片
                (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();
                userVM.CaptchaImage = captchaImage;
                HttpContext.Session.SetString(SessionKeys.CaptchaCode, captchaCode);

                return View(userVM);
            }
            else
            {
                HttpContext.Session.SetString(SessionKeys.UserVMEmail, userVM.Email);

                return RedirectToAction("ValidateQRCodeOTP", "User");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> SaveQRCodeOTP([FromForm] string QRCodeOTPSK)
        {
             await _userService.SaveQRCodeOTPAsync(QRCodeOTPSK);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ValidateQRCodeOTP()
        {
            string userVMEmail = HttpContext.Session.GetString(SessionKeys.UserVMEmail) ?? "";

            if (string.IsNullOrEmpty(userVMEmail))
                return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(await _userService.IsQRCodeOTPSecretKeyAsync(userVMEmail)))
            {
                await _userService.LoginSuccessfulAsync(userVMEmail);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateQRCodeOTP(UserViewModel userVM)
        {
            string userVMEmail = HttpContext.Session.GetString("UserVMEmail") ?? "";

            if (string.IsNullOrEmpty(userVMEmail))
                return RedirectToAction("Login", "User");

            List<string> result = await _userService.VerifyQRCodeOTPAsync(userVMEmail, userVM.ConfirmQRCodeOTP);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                await _userService.LoginSuccessfulAsync(userVMEmail);

                return RedirectToAction("Index", "Home");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(UserViewModel userVM)
        {
            await _userService.DeleteAccountAsync(userVM);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> IsLoggedIn()
        {
            bool isUserLoggedIn = await _userService.IsLoginAsync();
            return Json(isUserLoggedIn);
        }

        public async Task<IActionResult> GetFriends()
        {
            List<FriendshipViewModel> friendsVM = await _userService.GetAllFriendsAsync();
            return PartialView("_FriendsPartial", friendsVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
