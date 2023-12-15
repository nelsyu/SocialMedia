using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;

namespace SocialMedia.Controllers
{
    public class FriendController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFriendService _friendService;
        private readonly SocialMediaContext _dbContext;

        public FriendController(ILogger<HomeController> logger, IFriendService friendService, SocialMediaContext dbContext)
        {
            _logger = logger;
            _friendService = friendService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("GetAllFriends")]
        public async Task<IActionResult> GetAllFriends()
        {
            List<UserViewModel> usersVM = await _friendService.GetAllFriendsAsync();
            return PartialView("_FriendsPartial", usersVM);
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [Route("AddFriend")]
        public async Task<IActionResult> AddFriend(int userId2)
        {
            await _friendService.AddFriend(userId2);

            return Redirect($"/Profile?userId2={userId2}");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [Route("AcceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest(int userId2)
        {
            await _friendService.AcceptFriendRequest(userId2);

            return Redirect($"/Profile?userId2={userId2}");
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [Route("RejectFriendRequest")]
        public async Task<IActionResult> RejectFriendRequest(int userId2)
        {
            await _friendService.RejectFriendRequest(userId2);

            return Redirect($"/Profile?userId2={userId2}");
        }
    }
}
