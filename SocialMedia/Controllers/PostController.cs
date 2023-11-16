using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using SocialMedia.Filters;
using System.Diagnostics;
using Library.Extensions;

namespace SocialMedia.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly IReplyService _replyService;
        private readonly ITopicService _topicService;

        public PostController(ILogger<PostController> logger, IPostService postService, IReplyService replyService, ITopicService topicService)
        {
            _logger = logger;
            _postService = postService;
            _replyService = replyService;
            _topicService = topicService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyPost(int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
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
            PostViewModel postVM = new()
            {
                Topics = _topicService.GetAllTopics()
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

                postVM.Topics = _topicService.GetAllTopics();

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
            PostViewModel postVM = _postService.GetPost(postId);
            postVM.Topics = _topicService.GetAllTopics();

            return View(postVM);
        }

        [HttpPost]
        public IActionResult EditPost(PostViewModel postVM, int postId)
        {
            List<string> result = _postService.ValidatePost(postVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                postVM.Topics = _topicService.GetAllTopics();

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
