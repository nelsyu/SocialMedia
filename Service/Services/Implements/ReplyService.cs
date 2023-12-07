using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Service.Services.Interfaces;
using Service.ViewModels;
using Microsoft.EntityFrameworkCore;
using Library.Extensions;
using Library.Constants;

namespace Service.Services.Implements
{
    public class ReplyService : IReplyService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISession? _session;

        public ReplyService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _session = httpContextAccessor.HttpContext?.Session;
        }

        public async Task CreateReplyAsync(ReplyViewModel replyVM)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
            {
                replyVM.UserId = await _dbContext.Users
                    .Where(u => u.Username == sessionUserLoggedIn.Username)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
                replyVM.CreateDate = DateTime.Now;
            }

            var replyEnt = _mapper.Map<Reply>(replyVM);
            await _dbContext.Replies.AddAsync(replyEnt);
            await _dbContext.SaveChangesAsync();
        }

    }
}
