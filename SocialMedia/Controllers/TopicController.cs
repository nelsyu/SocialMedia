using Data.Entities;
using Library.Constants;
using Library.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using SocialMedia.Models;
using System.Diagnostics;

namespace SocialMedia.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly ISession? _session;

        public TopicController(ITopicService topicService, IPostService postService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _topicService = topicService;
            _postService = postService;
            _userService = userService;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public async Task<IActionResult> Index()
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            List<TopicViewModel> topicsVM = await _topicService.GetAllTopicsAsync();
            int? userId = sessionUserLoggedIn?.UserId;
            ViewData[ParameterKeys.HasRole] = await _userService.FindRole(userId, 2);

            return View(topicsVM);
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
        [HttpPost]
        public async Task<IActionResult> CreateTopic(TopicViewModel topicVM, string title)
        {
            await _topicService.CreateTopicAsync(topicVM, title);

            return RedirectToAction("Index", "Topic");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            await _topicService.DeleteTopicAsync(topicId);

            return RedirectToAction("Index", "Topic");
        }
    }
}
