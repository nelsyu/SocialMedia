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

        public PostController(ILogger<PostController> logger, IPostService postService, IUserService userService, IReplyService replyService)
        {
            _logger = logger;
            _postService = postService;
            _userService = userService;
            _replyService = replyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyPost(int page = 1)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Index", "Home");

            int pageSize = 5;
            var postVMs = _postService.GetMyPosts(page, pageSize);
            int totalPosts = _postService.GetTotalPostCount();
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
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(PostViewModel postVM)
        {
            if(ModelState.IsValid)
            {
                _postService.CreatePost(postVM);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        public IActionResult DeletePost(PostViewModel postVM)
        {
            _postService.DeletePost(postVM);

            return RedirectToAction("MyPost", "Post");
        }

        public IActionResult DetailPost(int postId)
        {
            var postVM = _postService.GetPost(postId);
            if (postVM == null)
                return NotFound();

            return View(postVM);
        }

        [HttpPost]
        public IActionResult AddReply(ReplyViewModel replyVM, int postId, string replyContent)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("DetailPost", "Post", new { postId });

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
