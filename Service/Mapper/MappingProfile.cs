using AutoMapper;
using Data.Entities;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostViewModel>()
                .ReverseMap();
            CreateMap<Reply, ReplyViewModel>()
                .ReverseMap();
            CreateMap<Topic, TopicViewModel>()
                .ReverseMap();
            CreateMap<User, UserViewModel>()
                .ReverseMap();
            CreateMap<Friendship, FriendshipViewModel>()
                .ReverseMap();
            CreateMap<Like, LikeViewModel>()
                .ReverseMap();
            CreateMap<Message, MessageViewModel>()
                .ReverseMap();
        }
    }
}
