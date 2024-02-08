using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using Library.Extensions;
using Library.Constants;
using Library.Models;
using System.Security.Claims;

namespace Service.Services.Implements
{
    public class TopicService : ITopicService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public TopicService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
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
            TopicViewModel topicVM = new()
            {
                UserId = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!),
                Title = title
            };

            var topicEnt = _mapper.Map<Topic>(topicVM);
            await _dbContext.AddAsync(topicEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteTopicAsync(int topicId)
        {
            List<Post> postsEnt = await _dbContext.Posts.Where(p => p.TopicId == topicId).ToListAsync();
                
            if(postsEnt.Count == 0)
            {
                Topic? topicEnt = await _dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
            
                _dbContext.Remove(topicEnt!);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
                return false;
        }
    }
}
