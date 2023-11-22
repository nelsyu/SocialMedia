using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Service.Services.Interfaces;
using Service.ViewModels;
using Service.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Service.Services.Implements
{
    public class ReplyService : IReplyService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserLoggedIn _userLoggedIn;

        public ReplyService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserLoggedIn userLoggedIn)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userLoggedIn = userLoggedIn;
        }

        public async Task CreateReplyAsync(ReplyViewModel replyVM)
        {
            replyVM.UserId = await _dbContext.Users
                .Where(u => u.Username == _userLoggedIn.Username)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
            replyVM.ReplyDate = DateTime.Now;

            var replyEnt = _mapper.Map<Reply>(replyVM);
            await _dbContext.Replies.AddAsync(replyEnt);
            await _dbContext.SaveChangesAsync();
        }

    }
}
