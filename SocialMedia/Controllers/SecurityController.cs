using Library.Constants;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace SocialMedia.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public SecurityController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("getCaptcha")]
        public async Task<IActionResult> GetCaptcha()
        {
            (byte[] captchaImage, string captchaCode) = await _userService.GenerateCaptchaImageAsync();
            HttpContext.Session.SetString(ParameterKeys.CaptchaCode, captchaCode);

            return File(captchaImage, "image/png");
        }
    }
}
