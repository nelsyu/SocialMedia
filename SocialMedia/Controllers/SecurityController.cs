using Lazy.Captcha.Core;
using Library.Constants;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace SocialMedia.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICaptcha _captcha;

        public SecurityController(ILogger<HomeController> logger, ICaptcha captcha)
        {
            _logger = logger;
            _captcha = captcha;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("generateCaptcha")]
        public IActionResult generateCaptcha()
        {
            string uId = Guid.NewGuid().ToString().Replace("-", "");
            CaptchaData captchaImage = _captcha.Generate(uId);
            var captcha = new {uId, img = captchaImage.Base64};

            return Ok(captcha);
        }
    }
}
