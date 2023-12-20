using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using SocialMedia.Filters;

namespace SocialMedia.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class SettingController : Controller
    {
        private readonly IUserService _userService;

        public SettingController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Settings()
        {
            return View();
        }

        [HttpGet("GetQRCodeOTP")]
        public async Task<IActionResult> GetQRCodeOTP()
        {
            (byte[] oTPQRCode, string secretKey) = await _userService.GetOTPQRCodeAsync();

            return Json(new { oTPQRCode = Convert.ToBase64String(oTPQRCode), oTPQRCodeSK = secretKey });
        }
    }
}