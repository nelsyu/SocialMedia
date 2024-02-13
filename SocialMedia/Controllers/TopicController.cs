using Data.Entities;
using Library.Models;
using Library.Constants;
using Library.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;
using Microsoft.AspNetCore.Authorization;

namespace SocialMedia.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly HttpContext _httpContext;

        public TopicController(ITopicService topicService, IHttpContextAccessor httpContextAccessor)
        {
            _topicService = topicService;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<IActionResult> Index()
        {
            ViewData[ParameterKeys.IsAdministrator] = _httpContext.User.IsInRole("2");

            return View();
        }

        public async Task<IActionResult> DetailTopic(int topicId)
        {
            ViewData[ParameterKeys.TopicId] = topicId;

            return View();
        }

        [AllowAnonymous]
        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics()
        {
            List<TopicViewModel> topicsVM = await _topicService.GetAllTopicsAsync();

            return Json(new { topicsVM });
        }

        [Authorize(Roles = "2")]
        [HttpPost("CreateTopic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTopic(string title)
        {
            await _topicService.CreateTopicAsync(title);

            return Ok();
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            bool deleteSuccessful = await _topicService.DeleteTopicAsync(topicId);

            return Ok(new { deleteSuccessful });
        }
    }
}
