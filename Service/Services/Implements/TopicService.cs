using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Extensions;

namespace Service.Services.Implements
{
    public class TopicService : ITopicService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopicService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<TopicViewModel> GetAllTopics()
        {
            List<Topic> topicEntL = _dbContext.Topics
                .Include(t => t.User)
                .ToList();
            List<TopicViewModel> topicVML = _mapper.Map<List<TopicViewModel>>(topicEntL);

            return topicVML;
        }

        public void CreateTopic(TopicViewModel topicVM, string title)
        {
            UserViewModel userVM = _httpContextAccessor.HttpContext?.Session.GetObject<UserViewModel>("UserNowVM") ?? new();

            topicVM.UserId = userVM.UserId;
            topicVM.Title = title;

            var topicEnt = _mapper.Map<Topic>(topicVM);
            _dbContext.Add(topicEnt);
            _dbContext.SaveChanges();
        }
    }
}
