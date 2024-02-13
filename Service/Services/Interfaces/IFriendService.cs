using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IFriendService
    {
        Task<List<OtherUserViewModel>> GetAllFriendsAsync();
        Task AddFriend(int userId2);
        Task AcceptFriendRequest(int userId2);
        Task RejectFriendRequest(int userId2);
        Task<int?> FriendshipStatus(int userId, int userId2);
    }
}
