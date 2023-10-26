using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using System.Diagnostics;

namespace SocialMedia.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IReplyService _replyService;
        private readonly ITopicService _topicService;

        public PostController(ILogger<PostController> logger, IPostService postService, IUserService userService, IReplyService replyService, ITopicService topicService)
        {
            _logger = logger;
            _postService = postService;
            _userService = userService;
            _replyService = replyService;
            _topicService = topicService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyPost(int page = 1)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            int pageSize = 5;
            List<PostViewModel> postVMs = _postService.GetMyPosts();
            int totalPosts = _postService.GetTotalPostCount(postVMs);
            postVMs = _postService.Paging(postVMs, page, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            // 傳遞資料到 View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;


            return View(postVMs);
        }

        public IActionResult CreatePost()
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            PostViewModel postVM = new()
            {
                TopicViewModels = _topicService.GetAllTopics()
            };

            return View(postVM);
        }

        [HttpPost]
        public IActionResult CreatePost(PostViewModel postVM)
        {
            _postService.CreatePost(postVM);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeletePost(PostViewModel postVM)
        {
            _postService.DeletePost(postVM);

            return RedirectToAction("MyPost", "Post");
        }

        public IActionResult DetailPost(int postId)
        {
            PostViewModel postVM = _postService.GetPost(postId);
            if (postVM == null)
                return NotFound();

            return View(postVM);
        }

        public IActionResult EditPost(int postId)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            PostViewModel postVM = _postService.GetPost(postId);
            postVM.TopicViewModels = _topicService.GetAllTopics();

            if (postVM == null)
                return NotFound();
            else
                return View(postVM);
        }

        [HttpPost]
        public IActionResult EditPost(PostViewModel postVM, int postId)
        {
            _postService.UpdatePost(postVM, postId);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AddReply(ReplyViewModel replyVM, int postId, string replyContent)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            replyVM.PostId = postId;
            replyVM.Content = replyContent;
            _replyService.CreateReply(replyVM);
            return RedirectToAction("DetailPost", "Post", new { postId });

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
