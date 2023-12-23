using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Models;
using SocialMedia.Filters;
using System.Diagnostics;
using Library.Extensions;
using Library.Constants;
using Data.Entities;
using Service.Services.Implements;

namespace SocialMedia.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IReplyService _replyService;
        private readonly IValidationService _validationService;
        private readonly ISession? _session;

        public PostController(IPostService postService, IReplyService replyService, IValidationService validationService, IHttpContextAccessor httpContextAccessor)
        {
            _postService = postService;
            _replyService = replyService;
            _validationService = validationService;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public IActionResult MyPost()
        {
            return View();
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        public async Task<IActionResult> CreatePost()
        {
            PostViewModel postVM = new();

            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel postVM)
        {
            List<(string key, string errorMessage)> errors = await _validationService.ValidatePostAsync(postVM);

            if (errors.Count == 0)
            {
                await _postService.CreatePostAsync(postVM);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var (key, errorMessage) in errors)
                {
                    ModelState.AddModelError(key, errorMessage);
                }

                return View(postVM);
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost("DeletePost")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _postService.DeletePostAsync(postId);

            return Ok();
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

            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostViewModel postVM, int postId)
        {
            List<(string key, string errorMessage)> errors = await _validationService.ValidatePostAsync(postVM);

            if (errors.Count == 0)
            {
                await _postService.UpdatePostAsync(postVM, postId);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var (key, errorMessage) in errors)
                {
                    ModelState.AddModelError(key, errorMessage);
                }

                return View(postVM);
            }
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost("AddReply")]
        public async Task<IActionResult> AddReply(ReplyViewModel replyVM)
        {
            await _replyService.CreateReplyAsync(replyVM);

            return Ok();
        }

        [HttpGet("GetPosts")]
        public async Task<IActionResult> GetPosts(string postsType = "all", int id = 0, int currentPage = 1)
        {
            if (currentPage < 1)
                return RedirectToAction("Index", "Home");

            int pageSize = 5;
            List<PostViewModel> postsVM = new();

            if (postsType == "all")
                postsVM = await _postService.GetAllPostsAsync();
            else if (postsType == "my")
                postsVM = await _postService.GetMyPostsAsync();
            else if (postsType == "user")
                postsVM = await _postService.GetMyPostsAsync(id);
            else if (postsType == "topic")
                postsVM = await _postService.GetAllPostsAsync(id);

            int totalPosts = postsVM.Count;
            postsVM = await _postService.PagingAsync(postsVM, currentPage, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            return Json(new { postsVM, currentPage, totalPages });
        }
    }
}
