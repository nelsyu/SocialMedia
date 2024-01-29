using Data.Entities;
using Library.Models;
using Library.Constants;
using Library.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;

namespace SocialMedia.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISession? _session;

        public TopicController(ITopicService topicService, IHttpContextAccessor httpContextAccessor)
        {
            _topicService = topicService;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public async Task<IActionResult> Index()
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            ViewData[ParameterKeys.IsAdministrator] = sessionUserLoggedIn?.RolesId?.Any(i => i == 2);

            return View();
        }

        public async Task<IActionResult> DetailTopic(int topicId)
        {
            ViewData[ParameterKeys.TopicId] = topicId;

            return View();
        }

        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics()
        {
            List<TopicViewModel> topicsVM = await _topicService.GetAllTopicsAsync();

            return Json(new { topicsVM });
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost("CreateTopic")]
        public async Task<IActionResult> CreateTopic(string title)
        {
            await _topicService.CreateTopicAsync(title);

            return Ok();
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            await _topicService.DeleteTopicAsync(topicId);

            return Ok();
        }
    }
}
