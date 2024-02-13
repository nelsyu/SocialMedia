using AutoMapper;
using Data.Entities;
using Service.ViewModels;

namespace Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Friendship, FriendshipViewModel>()
                .ReverseMap();
            CreateMap<FriendshipStatus, FriendshipStatusViewModel>()
                .ReverseMap();
            CreateMap<Like, LikeViewModel>()
                .ReverseMap();
            CreateMap<Message, MessageViewModel>()
                .ReverseMap();
            CreateMap<Notification, NotificationViewModel>()
                .ReverseMap();
            CreateMap<Post, PostViewModel>()
                .ReverseMap();
            CreateMap<Reply, ReplyViewModel>()
                .ReverseMap();
            CreateMap<Role,RoleViewModel>()
                .ReverseMap();
            CreateMap<Topic, TopicViewModel>()
                .ReverseMap();
            CreateMap<User, UserViewModel>()
                .ReverseMap();
            CreateMap<User, OtherUserViewModel>()
                .ReverseMap();
        }
    }
}
