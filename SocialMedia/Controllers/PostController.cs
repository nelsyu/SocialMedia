using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using SocialMedia.Filters;
using System.Diagnostics;
using Library.Extensions;
using Library.Constants;

namespace SocialMedia.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly IReplyService _replyService;
        private readonly ITopicService _topicService;
        private readonly ISession? _session;

        public PostController(ILogger<PostController> logger, IPostService postService, IReplyService replyService, ITopicService topicService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _postService = postService;
            _replyService = replyService;
            _topicService = topicService;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllPosts(int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
            int pageSize = 5;
            List<PostViewModel> postsVM = await _postService.GetAllPostsAsync();
            int totalPosts = postsVM.Count;
            postsVM = await _postService.PagingAsync(postsVM, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            return Json(new { posts = postsVM, currentPage, totalPages });
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> MyPost(int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");
            int pageSize = 5;
            List<PostViewModel> postsVM = await _postService.GetMyPostsAsync();
            int totalPosts = postsVM.Count;
            postsVM = await _postService.PagingAsync(postsVM, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            PageViewModel pageVM = new()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View((postsVM, pageVM));
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> CreatePost()
        {
            PostViewModel postVM = new()
            {
                Topics = await _topicService.GetAllTopicsAsync()
            };

            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel postVM)
        {
            List<string> result = await _postService.ValidatePostAsync(postVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                postVM.Topics = await _topicService.GetAllTopicsAsync();

                return View(postVM);
            }
            else
            {
                await _postService.CreatePostAsync(postVM);

                return RedirectToAction("Index", "Home");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> DeletePost(PostViewModel postVM)
        {
            await _postService.DeletePostAsync(postVM);

            return RedirectToAction("MyPost", "Post");
        }

        public async Task<IActionResult> DetailPost(int postId)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            TempData[ParameterKeys.LoggedInUsername] = sessionUserLoggedIn?.Username;
            PostViewModel postVM = await _postService.GetPostAsync(postId);
            ReplyViewModel replyVM = new();
            
            return View((postVM, replyVM));
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> EditPost(int postId)
        {
            PostViewModel postVM = await _postService.GetPostAsync(postId);
            postVM.Topics = await _topicService.GetAllTopicsAsync();

            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostViewModel postVM, int postId)
        {
            List<string> result = await _postService.ValidatePostAsync(postVM);

            if (result[0] != "")
            {
                ModelState.AddModelError(result[0], result[1]);

                postVM.Topics = await _topicService.GetAllTopicsAsync();

                return View(postVM);
            }
            else
            {
                await _postService.UpdatePostAsync(postVM, postId);

                return RedirectToAction("Index", "Home");
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public async Task<IActionResult> AddReply(ReplyViewModel replyVM)
        {
            await _replyService.CreateReplyAsync(replyVM);

            return RedirectToAction("DetailPost", "Post", new { replyVM.PostId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
