using AutoMapper;
using Data.Entities;
using Library.Constants;
using Library.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Service.ViewModels;

namespace Service.Services.Implements
{
    public class SignalRService : Hub
    {
        private readonly ISession? _session;
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;

        public SignalRService(IHttpContextAccessor httpContextAccessor, SocialMediaContext socialMediaContext, IMapper mapper)
        {
            _session = httpContextAccessor.HttpContext?.Session;
            _dbContext = socialMediaContext;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            string groupId = sessionUserLoggedIn?.UserId.ToString() ?? "";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task DirectMessage(string groupId, int userId2, string message)
        {
            DateTime sendTime = DateTime.Now;
            await Clients.Group(groupId).SendAsync("ReceiveMessage", message, sendTime.ToString("yyyy-MM-dd HH:mm:ss"));
            await SaveToDBAsync(userId2, message, sendTime);
        }

        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);

        private async Task SaveToDBAsync(int userId2, string message, DateTime sendTime)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
            {
                NotificationViewModel notificationVM = new()
                {
                    ReceiverUserId = userId2,
                    Message = message,
                    CreateTime = sendTime,
                    SenderUserId = sessionUserLoggedIn.UserId
                };

                Notification notificationEnt = _mapper.Map<Notification>(notificationVM);
                _dbContext.Notifications.Add(notificationEnt);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}