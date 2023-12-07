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
        private readonly ILogger<HomeController> _logger;
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly ISession? _session;

        public TopicController(ILogger<HomeController> logger, ITopicService topicService, IPostService postService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
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

        public async Task<IActionResult> DetailTopic(int topicId, int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
            int pageSize = 5;
            List<PostViewModel> postsVM = await _postService.GetAllPostsAsync(topicId);
            int totalPosts = postsVM.Count;
            postsVM = await _postService.PagingAsync(postsVM, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            PageViewModel pageVM = new()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            TempData["Id"] = topicId;

            return View((postsVM, pageVM));
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
        public async Task<IActionResult> DeleteTopic(TopicViewModel topicVM)
        {
            await _topicService.DeleteTopicAsync(topicVM);

            return RedirectToAction("Index", "Topic");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
