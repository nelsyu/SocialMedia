using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Service.Services.Interfaces;
using Service.ViewModels;
using Microsoft.EntityFrameworkCore;
using Library.Extensions;
using Library.Constants;
using Library.Models;
using System.Net.Http;

namespace Service.Services.Implements
{
    public class ReplyService : IReplyService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public ReplyService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task CreateReplyAsync(ReplyViewModel replyVM)
        {
            replyVM.UserId = await _dbContext.Users
                .Where(u => u.Email == _httpContext.User.Identity!.Name)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            replyVM.CreateDate = DateTime.Now;

            var replyEnt = _mapper.Map<Reply>(replyVM);
            await _dbContext.Replies.AddAsync(replyEnt);
            await _dbContext.SaveChangesAsync();
        }

    }
}
