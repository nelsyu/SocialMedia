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
                .ForMember(viewModel => viewModel.UserViewModel, option => option.MapFrom(entity => entity.User))
                .ForMember(viewModel => viewModel.ReplyViewModels, option => option.MapFrom(entity => entity.Replies))
                .ReverseMap();
            CreateMap<Reply, ReplyViewModel>()
                .ForMember(viewModel => viewModel.PostViewModel, option => option.MapFrom(entity => entity.Post))
                .ForMember(viewModel => viewModel.UserViewModel, option => option.MapFrom(entity => entity.User))
                .ReverseMap();
            CreateMap<Topic, TopicViewModel>()
                .ForMember(viewModel => viewModel.UserViewModel, option => option.MapFrom(entity => entity.User))
                .ReverseMap();
            CreateMap<User, UserViewModel>()
                .ForMember(viewModel => viewModel.PostsViewModels, option => option.MapFrom(entity => entity.Posts))
                .ForMember(viewModel => viewModel.RepliesViewModels, option => option.MapFrom(entity => entity.Replies))
                .ForMember(viewModel => viewModel.TopicsViewModels, option => option.MapFrom(entity => entity.Topics))
                .ReverseMap();
        }
    }
}
