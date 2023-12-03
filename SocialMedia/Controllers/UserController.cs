using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using SocialMedia.Models;
using System.Diagnostics;
using Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Data.Entities;
using Service.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Library.Constants;

namespace SocialMedia.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly SocialMediaContext _dbContext;
        private readonly ISession? _session;

        public UserController(ILogger<HomeController> logger, IUserService userService, IPostService postService, SocialMediaContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userService = userService;
            _postService = postService;
            _dbContext = dbContext;
            if (httpContextAccessor.HttpContext != null)
                _session = httpContextAccessor.HttpContext.Session;
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> Index(int userId2)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            List<PostViewModel> postsVM = await _postService.GetMyPostsAsync(userId2);
            TempData[ParameterKeys.LoggedInUserId] = sessionUserLoggedIn?.UserId;
            TempData[ParameterKeys.LoggedInUsername] = sessionUserLoggedIn?.Username;
            TempData[ParameterKeys.UserId2] = await _dbContext.Users
                .Where(u => u.UserId == userId2)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
            TempData[ParameterKeys.Username2] = await _dbContext.Users
                .Where(u => u.UserId == userId2)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();
            TempData[ParameterKeys.FriendshipStatus] = await _userService.FriendshipStatus(userId2);

            return View(postsVM);
        }

        public async Task<IActionResult> Register()
        {
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
            HttpContext.Session.SetString(ParameterKeys.CaptchaCode, captchaCode);

            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshCaptcha()
        {
            (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();
            HttpContext.Session.SetString(ParameterKeys.CaptchaCode, captchaCode);

            return Json(new { image = Convert.ToBase64String(captchaImage) });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            List<string> result = await _userService.ValidateLoginAsync(userVM, HttpContext.Session.GetString(ParameterKeys.CaptchaCode) ?? "");

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                // 生成 captcha 圖片
                (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();
                userVM.CaptchaImage = captchaImage;
                HttpContext.Session.SetString(ParameterKeys.CaptchaCode, captchaCode);

                return View(userVM);
            }
            else
            {
                int userId = await _userService.FindUserId(userVM.Email);
                TempData[ParameterKeys.LoggedInUserId] = userId;

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
            List<UserViewModel> usersVM = await _userService.GetAllFriendsAsync();
            return PartialView("_FriendsPartial", usersVM);
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> FriendAdd(int userId2)
        {
            await _userService.FriendAdd(userId2);

            return Redirect($"/User?userId2={userId2}");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> FriendConfirm(int userId2)
        {
            await _userService.FriendConfirm(userId2);

            return Redirect($"/User?userId2={userId2}");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> FriendDeny(int userId2)
        {
            await _userService.FriendDeny(userId2);

            return Redirect($"/User?userId2={userId2}");
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
