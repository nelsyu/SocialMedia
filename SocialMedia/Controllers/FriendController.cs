using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;
using Service.ViewModels;
using SocialMedia.Filters;

namespace SocialMedia.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpGet("GetAllFriends")]
        public async Task<IActionResult> GetAllFriends()
        {
            List<OtherUserViewModel> friendsVM = await _friendService.GetAllFriendsAsync();
            return PartialView("_FriendsPartial", friendsVM);
        }

        [HttpPost("AddFriend")]
        public async Task<IActionResult> AddFriend(int userId2)
        {
            await _friendService.AddFriend(userId2);

            return Ok();
        }

        [HttpPost("AcceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest(int userId2)
        {
            await _friendService.AcceptFriendRequest(userId2);

            return Ok();
        }

        [HttpPost("RejectFriendRequest")]
        public async Task<IActionResult> RejectFriendRequest(int userId2)
        {
            await _friendService.RejectFriendRequest(userId2);

            return Ok();
        }
    }
}
