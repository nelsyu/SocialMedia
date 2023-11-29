using Microsoft.AspNetCore.SignalR;
using Service.Extensions;

namespace Library.Extensions
{
    public class ChatHub : Hub
    {
        private readonly UserLoggedIn _userLoggedIn;

        public ChatHub(UserLoggedIn userLoggedIn)
        {
            _userLoggedIn = userLoggedIn;
        }

        public override async Task OnConnectedAsync()
        {
            string groupId = _userLoggedIn.UserId.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task DirectMessage(string groupId, string user, string message)
            => await Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);

        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
    }
}