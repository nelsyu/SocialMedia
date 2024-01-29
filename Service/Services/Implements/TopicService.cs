using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using Library.Extensions;
using Library.Constants;
using Library.Models;

namespace Service.Services.Implements
{
    public class TopicService : ITopicService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISession? _session;

        public TopicService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public async Task<List<TopicViewModel>> GetAllTopicsAsync()
        {
            List<Topic> topicsEnt = await _dbContext.Topics
                .Include(t => t.User)
                .ToListAsync();
            List<TopicViewModel> topicsVM = _mapper.Map<List<TopicViewModel>>(topicsEnt);

            return topicsVM;
        }

        public async Task CreateTopicAsync(string title)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            
            if (sessionUserLoggedIn != null)
            {
                TopicViewModel topicVM = new()
                {
                    UserId = sessionUserLoggedIn.UserId,
                    Title = title
                };

                var topicEnt = _mapper.Map<Topic>(topicVM);
                await _dbContext.AddAsync(topicEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteTopicAsync(int topicId)
        {
            Topic? topicEnt = await _dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
            if (topicEnt != null)
            {
                List<Post> postsEnt = await _dbContext.Posts.Where(p => p.TopicId == topicId).ToListAsync();
                
                if(postsEnt.Count > 0)
                {
                    foreach (Post postEnt in postsEnt)
                        postEnt.TopicId = 1;
                }
                _dbContext.Remove(topicEnt);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
