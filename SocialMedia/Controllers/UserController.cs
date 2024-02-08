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
using Library.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SocialMedia.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IFriendService _friendService;
        private readonly SocialMediaContext _dbContext;
        private readonly ICaptcha _captcha;
        private readonly HttpContext _httpContext;

        public UserController(IUserService userService, IValidationService validationService, IFriendService friendService, SocialMediaContext dbContext, IHttpContextAccessor httpContextAccessor, ICaptcha captcha)
        {
            _userService = userService;
            _validationService = validationService;
            _friendService = friendService;
            _dbContext = dbContext;
            _captcha = captcha;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string userName)
        {
            int userId = await _userService.FindUserIdAsync(userName);

            return RedirectToAction("Profile", "User", new { userId2 = userId });
        }

        public async Task<IActionResult> Profile(int userId2)
        {
            TempData[ParameterKeys.UserId2] = await _dbContext.Users
                .Where(u => u.Id == userId2)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            return View();
        }

        public async Task<IActionResult> Register()
        {
            if (_httpContext!.User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
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

        public async Task<IActionResult> Login()
        {
            if (_httpContext!.User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            List<(string key, string errorMessage)> errors = await _validationService.ValidateLoginAsync(userVM);

            if (errors.Count == 0)
            {
                int userId = await _userService.FindUserIdAsync(userVM.Email);

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

        public async Task<IActionResult> ValidateQRCodeOTP(int userId)
        {


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
                UserLoggedIn? userInfo = await _userService.GetUserInfoAsync(userId);

                if (userInfo != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.Email!),
                        new Claim(ParameterKeys.UserIdLoggedIn, userInfo.UserId.ToString()),
                        new Claim(ParameterKeys.UsernameLoggedIn, userInfo.Username!),
                    };

                    foreach (var temp in userInfo.RolesId!)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, temp.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }

                return RedirectToAction("Index", "Home");
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
                UserLoggedIn? userInfo = await _userService.GetUserInfoAsync(qRCodeOTPVM.UserId);

                if (userInfo != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.Email!),
                        new Claim(ParameterKeys.UserIdLoggedIn, userInfo.UserId.ToString()),
                        new Claim(ParameterKeys.UsernameLoggedIn, userInfo.Username!),
                    };

                    foreach (var temp in userInfo.RolesId!)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, temp.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }

                return RedirectToAction("Index", "Home");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

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

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [Route("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            List<NotificationViewModel> notificationsVM = await _userService.GetNotificationAsync();
            
            return PartialView("_NoticesPartial", notificationsVM);
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(int? userId)
        {
            UserLoggedIn? userInfo;
            if (userId == null)
                userInfo = await _userService.GetUserInfoAsync(Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)));
            else
                userInfo = await _userService.GetUserInfoAsync(userId.Value);
            return Json(new { userInfo });
        }

        [HttpGet("GetFriendshipStatus")]
        public async Task<IActionResult> GetFriendshipStatus(int userId, int userId2)
        {
            int? friendshipStatus = await _friendService.FriendshipStatus(userId, userId2);

            return Json(new { friendshipStatus });
        }
    }
}
