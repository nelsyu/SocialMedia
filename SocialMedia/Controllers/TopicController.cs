using Microsoft.AspNetCore.Mvc;
using Service.Services.Implements;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
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
            List<TopicViewModel> topicVML = _topicService.GetAllTopics();
            return View(topicVML);
        }

        public IActionResult DetailTopic(int topicId, int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
            int pageSize = 5;
            List<PostViewModel> postVML = _postService.GetAllPosts(topicId);
            int totalPosts = postVML.Count;
            postVML = _postService.Paging(postVML, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            PageViewModel pageVM = new()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            TempData["TopicId"] = topicId;

            return View((postVML, pageVM));
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public IActionResult CreateTopic(TopicViewModel topicVM, string title)
        {
            _topicService.CreateTopic(topicVM, title);

            return RedirectToAction("Index", "Topic");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public IActionResult DeleteTopic(TopicViewModel topicVM)
        {
            _topicService.DeleteTopic(topicVM);

            return RedirectToAction("Index", "Topic");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
