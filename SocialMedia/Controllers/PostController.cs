using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using System.Diagnostics;
using Library.Extensions;

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

        public IActionResult MyPost(int currentPage = 1)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            int pageSize = 5;
            List<PostViewModel> postVML = _postService.GetMyPosts();
            int totalPosts = postVML.Count;
            postVML = _postService.Paging(postVML, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            PageViewModel pageVM = new()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View((postVML, pageVM));
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
            List<string> result = _postService.ValidatePost(postVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                postVM.TopicViewModels = _topicService.GetAllTopics();

                return View(postVM);
            }
            else
            {
                _postService.CreatePost(postVM);

                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeletePost(PostViewModel postVM)
        {
            _postService.DeletePost(postVM);

            return RedirectToAction("MyPost", "Post");
        }

        public IActionResult DetailPost(int postId)
        {
            PostViewModel postVM = _postService.GetPost(postId);
            ReplyViewModel replyVM = new();
            
            return View((postVM, replyVM));
        }

        public IActionResult EditPost(int postId)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            PostViewModel postVM = _postService.GetPost(postId);
            postVM.TopicViewModels = _topicService.GetAllTopics();

            return View(postVM);
        }

        [HttpPost]
        public IActionResult EditPost(PostViewModel postVM, int postId)
        {
            List<string> result = _postService.ValidatePost(postVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                postVM.TopicViewModels = _topicService.GetAllTopics();

                return View(postVM);
            }
            else
            {
                _postService.UpdatePost(postVM, postId);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult AddReply(ReplyViewModel replyVM)
        {
            if (!_userService.IsLogin())
                return RedirectToAction("Login", "User");

            _replyService.CreateReply(replyVM);

            return RedirectToAction("DetailPost", "Post", new { replyVM.PostId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
