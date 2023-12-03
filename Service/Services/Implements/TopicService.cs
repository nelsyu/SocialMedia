using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using Service.Extensions;
using Library.Extensions;
using Library.Constants;

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
            if(httpContextAccessor.HttpContext != null)
                _session = httpContextAccessor.HttpContext.Session;
        }

        public async Task<List<TopicViewModel>> GetAllTopicsAsync()
        {
            List<Topic> topicsEnt = await _dbContext.Topics
                .Include(t => t.User)
                .ToListAsync();
            List<TopicViewModel> topicsVM = _mapper.Map<List<TopicViewModel>>(topicsEnt);

            return topicsVM;
        }

        public async Task CreateTopicAsync(TopicViewModel topicVM, string title)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            if (sessionUserLoggedIn != null)
                topicVM.UserId = sessionUserLoggedIn.UserId;
            topicVM.Title = title;

            var topicEnt = _mapper.Map<Topic>(topicVM);
            await _dbContext.AddAsync(topicEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTopicAsync(TopicViewModel topicVM)
        {
            Topic? topicEnt = await _dbContext.Topics.FirstOrDefaultAsync(t => t.TopicId == topicVM.TopicId);
            if (topicEnt != null)
            {
                List<Post> postsEnt = await _dbContext.Posts.Where(p => p.TopicId == topicVM.TopicId).ToListAsync();
                foreach (Post postEnt in postsEnt)
                {
                    postEnt.TopicId = 0;
                }
                _dbContext.Remove(topicEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
