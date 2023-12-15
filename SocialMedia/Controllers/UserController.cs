using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using SocialMedia.Models;
using System.Diagnostics;
using Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Data.Entities;
using Library.Constants;
using Lazy.Captcha.Core;

namespace SocialMedia.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly SocialMediaContext _dbContext;
        private readonly ISession? _session;
        private readonly ICaptcha _captcha;

        public UserController(ILogger<HomeController> logger, IUserService userService, IFriendService friendService, SocialMediaContext dbContext, IHttpContextAccessor httpContextAccessor, ICaptcha captcha)
        {
            _logger = logger;
            _userService = userService;
            _friendService = friendService;
            _dbContext = dbContext;
            _session = httpContextAccessor.HttpContext?.Session;
            _captcha = captcha;
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [Route("Profile")]
        public async Task<IActionResult> Profile(int userId2)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            TempData[ParameterKeys.LoggedInUserId] = sessionUserLoggedIn?.UserId;
            TempData[ParameterKeys.LoggedInUsername] = sessionUserLoggedIn?.Username;
            TempData[ParameterKeys.UserId2] = await _dbContext.Users
                .Where(u => u.Id == userId2)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            TempData[ParameterKeys.Username2] = await _dbContext.Users
                .Where(u => u.Id == userId2)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();
            TempData[ParameterKeys.FriendshipStatus] = await _friendService.FriendshipStatus(userId2);

            return View();
        }

        public async Task<IActionResult> Register()
        {
            if (await _userService.IsLoginAsync())
                return RedirectToAction("Index", "Home");

            return View();
        }

        public async Task<IActionResult> Login()
        {
            if (await _userService.IsLoginAsync())
                return RedirectToAction("Index", "Home");

            return View();
        }

        public async Task<IActionResult> ValidateQRCodeOTP()
        {
            var userId = TempData[ParameterKeys.LoggedInUserId];

            if (userId == null)
                return RedirectToAction("Login", "User");

            TempData[ParameterKeys.LoggedInUserId] = userId;

            if (string.IsNullOrEmpty(await _userService.IsQRCodeOTPSecretKeyAsync((int)userId)))
            {
                await _userService.LoginSuccessfulAsync((int)userId);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("GenerateCaptcha")]
        public IActionResult GenerateCaptcha()
        {
            string uId = Guid.NewGuid().ToString().Replace("-", "");
            CaptchaData captchaImage = _captcha.Generate(uId);
            var captcha = new { uId, img = captchaImage.Base64 };

            return Ok(captcha);
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

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            List<string> result = await _userService.ValidateLoginAsync(userVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                int userId = await _userService.FindUserId(userVM.Email);
                TempData[ParameterKeys.LoggedInUserId] = userId;

                return RedirectToAction("ValidateQRCodeOTP", "User");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateQRCodeOTP(UserViewModel userVM)
        {
            var userId = TempData[ParameterKeys.LoggedInUserId];

            if (userId == null)
                return RedirectToAction("Login", "User");

            List<string> result = await _userService.VerifyQRCodeOTPAsync((int)userId, userVM.ConfirmQRCodeOTP);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(userVM);
            }
            else
            {
                await _userService.LoginSuccessfulAsync((int)userId);

                return RedirectToAction("Index", "Home");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> SaveQRCodeOTP([FromForm] string QRCodeOTPSK)
        {
             await _userService.SaveQRCodeOTPAsync(QRCodeOTPSK);

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

        public async Task<IActionResult> ReceiveMessage()
        {
            List<NotificationViewModel> notificationsVM = await _userService.GetNotificationsAsync();
            
            return PartialView("_NoticesPartial", notificationsVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
