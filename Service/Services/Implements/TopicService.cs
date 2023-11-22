using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using Service.Extensions;

namespace Service.Services.Implements
{
    public class TopicService : ITopicService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserLoggedIn _userLoggedIn;

        public TopicService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserLoggedIn userLoggedIn)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userLoggedIn = userLoggedIn;
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
            topicVM.UserId = _userLoggedIn.UserId;
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
