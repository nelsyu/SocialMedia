using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using System.Linq.Expressions;
using Library.Extensions;
using Library.Constants;
using Library.Models;
using System.Security.Claims;

namespace Service.Services.Implements
{
    public class PostService : IPostService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public PostService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<PostViewModel> GetPostAsync(int postId)
        {
            Post? postEnt = await _dbContext.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.User)
                .Include(p => p.Replies).ThenInclude(r => r.User)
                .FirstOrDefaultAsync();

            var postVM = _mapper.Map<PostViewModel>(postEnt);

            return postVM;
        }

        public async Task<List<PostViewModel>> GetAllPostsAsync()
        {
            return await GetPostsAsync(p => true);
        }

        public async Task<List<PostViewModel>> GetAllPostsAsync(int topicId)
        {
            return await GetPostsAsync(p => p.TopicId == topicId);
        }

        public async Task<List<PostViewModel>> GetMyPostsAsync()
        {
            return await GetPostsAsync(p => p.User != null && p.User.Id == Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!));
        }

        public async Task<List<PostViewModel>> GetMyPostsAsync(int userId)
        {
            return await GetPostsAsync(p => p.User != null && p.User.Id == userId);
        }

        public Task<List<PostViewModel>> PagingAsync(List<PostViewModel> postsVM, int page, int pageSize)
        {
            int skipPosts = (page - 1) * pageSize;

            postsVM = postsVM
                .Skip(skipPosts)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(postsVM);
        }

        public async Task CreatePostAsync(PostViewModel postVM)
        {
            postVM.UserId = Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!);
            postVM.CreateDate = DateTime.Now;
            postVM.EditDate = DateTime.Now;

            var postEnt = _mapper.Map<Post>(postVM);
            _dbContext.Posts.Add(postEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int postId)
        {
            Post? postEnt = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (postEnt != null)
            {
                _dbContext.Posts.Remove(postEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdatePostAsync(PostViewModel postVM, int postId)
        {
            var postEnt = await _dbContext.Posts.FindAsync(postId);

            if (postEnt != null)
            {
                postEnt.TopicId = postVM.TopicId;
                postEnt.Title = postVM.Title;
                postEnt.Content = postVM.Content;
                postEnt.EditDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }
        }

        #region private methods
        private async Task<List<PostViewModel>> GetPostsAsync(Expression<Func<Post, bool>> condition)
        {
            List<Post> postsEnt = await _dbContext.Posts
                .Where(condition)
                .OrderByDescending(p => p.CreateDate)
                .Include(p => p.User)
                .ToListAsync();

            var postsVM = _mapper.Map<List<PostViewModel>>(postsEnt);

            return postsVM;
        }
        #endregion
    }
}
