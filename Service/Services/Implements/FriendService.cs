using AutoMapper;
using Data.Entities;
using Library.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using Library.Extensions;
using Library.Models;
using System.Net.Http;
using System.Security.Claims;

namespace Service.Services.Implements
{
    public class FriendService : IFriendService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public FriendService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<UserViewModel>> GetAllFriendsAsync()
        {
            int userIdLoggedIn = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!);

            List<Friendship>? friendshipsEnt  = await _dbContext.Friendships
                .Where(f => (f.FriendshipStatusId == 1) && ((f.UserId1 == userIdLoggedIn) || (f.UserId2 == userIdLoggedIn)))
                .ToListAsync();

            List<FriendshipViewModel> friendshipsVM = _mapper.Map<List<FriendshipViewModel>>(friendshipsEnt);
            List<UserViewModel> usersVM = new List<UserViewModel>();

            foreach (var friendshipVM in friendshipsVM)
            {
                int? userId2 = friendshipVM.UserId1 == userIdLoggedIn ? friendshipVM.UserId2 : friendshipVM.UserId1;
                User? userEnt = await _dbContext.Users.FindAsync(userId2);
                UserViewModel userVM = _mapper.Map<UserViewModel>(userEnt);
                usersVM.Add(userVM);
            }

            return usersVM;
        }

        public async Task AddFriend(int userId2)
        {
            FriendshipViewModel friendShipVM = new FriendshipViewModel
            {
                UserId1 = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!),
                UserId2 = userId2,
                FriendshipStatusId = 2,
                CreateTime = DateTime.Now
            };

            Friendship friendshipEnt = _mapper.Map<Friendship>(friendShipVM);
            _dbContext.Friendships.Add(friendshipEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptFriendRequest(int userId2)
        {
            int userLoggedIn = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!);

            Friendship? friendshipEnt =  await _dbContext.Friendships.Where(f => (f.UserId1 == userId2 && f.UserId2 == userLoggedIn)).FirstOrDefaultAsync();
            if (friendshipEnt != null)
            {
                friendshipEnt.FriendshipStatusId = 1;
                _dbContext.Friendships.Update(friendshipEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RejectFriendRequest(int userId2)
        {
            Notification? notificationEnt = null;
            int userLoggedIn = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!);

            Friendship? friendshipEnt = await _dbContext.Friendships
                .Where(f => (f.UserId1 == userLoggedIn && f.UserId2 == userId2) || (f.UserId1 == userId2 && f.UserId2 == userLoggedIn))
                .FirstOrDefaultAsync();

            if (friendshipEnt != null)
            {
                _dbContext.Friendships.Remove(friendshipEnt);
                await _dbContext.SaveChangesAsync();
            }

            notificationEnt = await _dbContext.Notifications
                .Where(n => n.ReceiverUserId == userId2 && n.Message == _httpContext.User.FindFirstValue(ParameterKeys.UsernameLoggedIn) + " want to be your friend!")
                .FirstOrDefaultAsync();

            if (notificationEnt != null)
            {
                _dbContext.Notifications.Remove(notificationEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int?> FriendshipStatus(int userId, int userId2)
        {
            Friendship? friendshipEnt = null;

            friendshipEnt = await _dbContext.Friendships
                .FirstOrDefaultAsync(f => (f.UserId1 == userId && f.UserId2 == userId2));

            if (friendshipEnt != null)
                return friendshipEnt.FriendshipStatusId;

            friendshipEnt = await _dbContext.Friendships
                .FirstOrDefaultAsync(f => (f.UserId1 == userId2 && f.UserId2 == userId));

            if (friendshipEnt != null)
            {
                if (friendshipEnt.FriendshipStatusId == 2)
                    return friendshipEnt.FriendshipStatusId + 1;
                else
                    return friendshipEnt.FriendshipStatusId;
            }

            return null;
        }
    }
}
