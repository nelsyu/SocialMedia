using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using SocialMedia.Models;
using System.Diagnostics;

namespace SocialMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IPostService postService, IUserService userService)
        {
            _logger = logger;
            _postService = postService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> Settings()
        {
            (byte[] oTPQRCode, string secretKey) = await _userService.GenerateOTPQRCodeAsync();

            OTPViewModel oTPVM = new OTPViewModel
            {
                OTPQRCode = oTPQRCode,
                OTPQRCodeSK = secretKey
            };

            return View(oTPVM);
        }


        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> GenerateOTPQRCode()
        {
            (byte[] oTPQRCode, string secretKey) = await _userService.GenerateOTPQRCodeAsync();

            return Json(new { oTPQRCode = Convert.ToBase64String(oTPQRCode), oTPQRCodeSK = secretKey });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}