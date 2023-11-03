using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
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
    public class ReplyService : IReplyService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReplyService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void CreateReply(ReplyViewModel replyVM)
        {
            UserViewModel userNowVM = _httpContextAccessor.HttpContext?.Session.GetObject<UserViewModel>("UserNowVM") ?? new();

            replyVM.UserId = _dbContext.Users
                .Where(u => u.Username == userNowVM.Username)
                .Select(u => u.UserId)
                .FirstOrDefault();
            replyVM.ReplyDate = DateTime.Now;

            var replyEnt = _mapper.Map<Reply>(replyVM);
            _dbContext.Replies.Add(replyEnt);
            _dbContext.SaveChanges();
        }
    }
}
