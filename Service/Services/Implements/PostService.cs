using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Extensions;
using System.Linq.Expressions;

namespace Service.Services.Implements
{
    public class PostService : IPostService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public PostViewModel GetPost(int postId)
        {
            Post? postEnt = _dbContext.Posts
                .Where(p => p.PostId == postId)
                .Include(p => p.User)
                .Include(p => p.Replies).ThenInclude(r => r.User)
                .FirstOrDefault();

            var postVM = _mapper.Map<PostViewModel>(postEnt);

            return postVM;
        }

        public List<PostViewModel> GetAllPosts()
        {
            return GetPosts(p => true);
        }

        public List<PostViewModel> GetAllPosts(int topicId)
        {
            return GetPosts(p => p.TopicId == topicId);
        }

        public List<PostViewModel> GetAllPosts(UserViewModel userNowVM)
        {
            return GetPosts(p => p.User != null && p.User.UserId == userNowVM.UserId);
        }

        public List<PostViewModel> Paging(List<PostViewModel> postVML, int page, int pageSize)
        {
            int skipPosts = (page - 1) * pageSize;
            
            postVML = postVML
                .Skip(skipPosts)
                .Take(pageSize)
                .ToList();

            return postVML;
        }

        public List<string> ValidatePost(PostViewModel postVM)
        {
            bool isTopicIdInvalid = !_dbContext.Topics.Any(t => t.TopicId == postVM.TopicId);
            bool isTitleInvalid = string.IsNullOrEmpty(postVM.Title);
            bool isContentInvalid = string.IsNullOrEmpty(postVM.Content);

            List<string> result = new();

            if (isTopicIdInvalid)
            {
                result.Add("TopicId");
                result.Add("TopicId is invalid.");
            }
            else if (isTitleInvalid)
            {
                result.Add("Title");
                result.Add("Title is invalid.");
            }
            else if (isContentInvalid)
            {
                result.Add("Content");
                result.Add("Content is invalid");
            }
            else
            {
                result.Add("");
            }

            return result;
        }

        public void CreatePost(PostViewModel postVM)
        {
            UserViewModel? userNowVM = _httpContextAccessor.HttpContext?.Session.GetObject<UserViewModel>("UserNowVM");
            
            if(userNowVM != null)
            {
                postVM.UserId = userNowVM.UserId;
                postVM.PostDate = DateTime.Now;
                postVM.LastEditDate = DateTime.Now;

                var postEnt = _mapper.Map<Post>(postVM);
                _dbContext.Posts.Add(postEnt);
                _dbContext.SaveChanges();
            }
        }

        public void UpdatePost(PostViewModel postVM, int postId)
        {
            var postEnt = _dbContext.Posts.Find(postId);

            if (postEnt != null)
            {
                postEnt.TopicId = postVM.TopicId;
                postEnt.Title = postVM.Title;
                postEnt.Content = postVM.Content;
                postEnt.LastEditDate = DateTime.Now;

                _dbContext.SaveChanges();
            }
        }

        public void DeletePost(PostViewModel postVM)
        {
            var postEnt = _dbContext.Posts.FirstOrDefault(p => p.PostId == postVM.PostId);

            if (postEnt != null)
            {
                _dbContext.Posts.Remove(postEnt);
                _dbContext.SaveChanges();
            }
        }

        #region private methods
        private List<PostViewModel> GetPosts(Expression<Func<Post, bool>> condition)
        {
            List<Post> postL = _dbContext.Posts
                .Where(condition)
                .OrderByDescending(p => p.PostDate)
                .Include(p => p.User)
                .ToList();

            List<PostViewModel> postVML = _mapper.Map<List<PostViewModel>>(postL);

            return postVML;
        }
        #endregion
    }
}
