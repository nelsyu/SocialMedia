﻿using Data.Entities;
using Library.Extensions;
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
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public TopicController(ILogger<HomeController> logger, ITopicService topicService, IPostService postService, IUserService userService)
        {
            _logger = logger;
            _topicService = topicService;
            _postService = postService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            List<TopicViewModel> topicsVM = await _topicService.GetAllTopicsAsync();
            string userVMEmail = HttpContext.Session.GetString(ParameterKeys.UserVMEmail) ?? "";
            ViewData[ParameterKeys.HasRole] = await _userService.FindRole(userVMEmail, 2);

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

            TempData["TopicId"] = topicId;

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
