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
            CreateMap<Post, PostViewModel>();
            CreateMap<Reply, ReplyViewModel>();
            CreateMap<Topic, TopicViewModel>();
            CreateMap<User, UserViewModel>();
            CreateMap<Friendship, FriendshipViewModel>();
            CreateMap<Like, LikeViewModel>();
            CreateMap<Message, MessageViewModel>();
        }
    }
}
