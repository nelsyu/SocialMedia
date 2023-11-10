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
using Service.Extensions;

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

        public void CreateReply(ReplyViewModel replyVM)
        {
            replyVM.UserId = _dbContext.Users
                .Where(u => u.Username == _userLoggedIn.Username)
                .Select(u => u.UserId)
                .FirstOrDefault();
            replyVM.ReplyDate = DateTime.Now;

            var replyEnt = _mapper.Map<Reply>(replyVM);
            _dbContext.Replies.Add(replyEnt);
            _dbContext.SaveChanges();
        }
    }
}
