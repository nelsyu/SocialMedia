using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Extensions;
using Service.Services.Implements;
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

        public async Task<IActionResult> Index(int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
            int pageSize = 5;
            List<PostViewModel> postVML = await _postService.GetAllPostsAsync();
            int totalPosts = postVML.Count;
            postVML = await _postService.PagingAsync(postVML, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            PageViewModel pageVM = new()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages
            };
            return View((postVML, pageVM));
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