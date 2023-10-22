using Microsoft.AspNetCore.Mvc;
using Service.Services.Implements;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using System.Diagnostics;

namespace SocialMedia.Controllers
{
    public class TopicController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITopicService _topicService;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public TopicController(ILogger<HomeController> logger, ITopicService topicService, IUserService userService, IPostService postService)
        {
            _logger = logger;
            _topicService = topicService;
            _userService = userService;
            _postService = postService;
        }

        public IActionResult Index()
        {
            List<TopicViewModel> topicVMs = _topicService.GetAllTopics();
            return View(topicVMs);
        }

        [HttpPost]
        public IActionResult CreateTopic(TopicViewModel topicVM, string title)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            _topicService.CreateTopic(topicVM, title);

            return RedirectToAction("Index", "Topic");
        }

        public IActionResult DetailTopic(int topicId, int page = 1)
        {
            if (page < 1)
                return NotFound();
            int pageSize = 5;
            List<PostViewModel> postVMs = _postService.GetAllPosts(topicId);
            int totalPosts = _postService.GetTotalPostCount(postVMs);
            postVMs = _postService.Paging(postVMs, page, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            // 傳遞資料到 View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TopicId = topicId;

            return View(postVMs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
