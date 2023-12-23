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
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IFriendService _friendService;
        private readonly SocialMediaContext _dbContext;
        private readonly ISession? _session;
        private readonly ICaptcha _captcha;

        public UserController(IUserService userService, IValidationService validationService, IFriendService friendService, SocialMediaContext dbContext, IHttpContextAccessor httpContextAccessor, ICaptcha captcha)
        {
            _userService = userService;
            _validationService = validationService;
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

        public async Task<IActionResult> ValidateQRCodeOTP(int userId)
        {
            if (userId == 0)
                return RedirectToAction("Login", "User");

            QRCodeOTPViewModel QRCodeOTPVM = new QRCodeOTPViewModel
            {
                UserId = userId,
                QRCodeOTP = ""
            };

            if (await _userService.HasQRCodeOTPSKAsync(userId))
            {
                return View(QRCodeOTPVM);
            }
            else
            { 
                await _userService.LoginSuccessfulAsync(userId);

                return RedirectToAction("Index", "Home");
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
            List<(string key, string errorMessage)> errors = await _validationService.ValidateRegisterAsync(userVM);

            if (errors.Count == 0)
            {
                await _userService.RegisterAsync(userVM);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var (key, errorMessage) in errors)
                {
                    ModelState.AddModelError(key, errorMessage);
                }

                return View(userVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            List<(string key, string errorMessage)> errors = await _validationService.ValidateLoginAsync(userVM);

            if (errors.Count == 0)
            {
                int userId = await _userService.FindUserId(userVM.Email);

                return RedirectToAction("ValidateQRCodeOTP", "User", new { userId });
            }
            else
            {
                foreach (var (key, errorMessage) in errors)
                {
                    ModelState.AddModelError(key, errorMessage);
                }

                return View(userVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateQRCodeOTP([FromForm] QRCodeOTPViewModel qRCodeOTPVM)
        {
            if (qRCodeOTPVM.UserId == 0)
                return RedirectToAction("Login", "User");

            List<string> result = await _userService.VerifyQRCodeOTPAsync(qRCodeOTPVM.UserId, qRCodeOTPVM.QRCodeOTP);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                return View(qRCodeOTPVM);
            }
            else
            {
                await _userService.LoginSuccessfulAsync(qRCodeOTPVM.UserId);

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

        [Route("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            List<NotificationViewModel> notificationsVM = await _userService.GetNotificationAsync();
            
            return PartialView("_NoticesPartial", notificationsVM);
        }
    }
}
