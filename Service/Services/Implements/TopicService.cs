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
            List<Topic> topics = _dbContext.Topics
                .Include(t => t.User)
                .ToList();
            List<TopicViewModel> topicVMs = _mapper.Map<List<TopicViewModel>>(topics);
            return topicVMs;
        }

        public void CreateTopic(TopicViewModel topicVM, string title)
        {
            string username = _httpContextAccessor.HttpContext?.Session.GetString("Username") ?? string.Empty;

            topicVM.UserId = _dbContext.Users.Where(u => u.Username == username).Select(u => u.UserId).FirstOrDefault();
            topicVM.Title = title;

            var topicMap = _mapper.Map<Topic>(topicVM);
            _dbContext.Add(topicMap);
            _dbContext.SaveChanges();
        }
    }
}
