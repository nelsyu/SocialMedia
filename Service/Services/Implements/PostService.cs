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
            var post = _dbContext.Posts
                .Where(p => p.PostId == postId)
                .Include(p => p.User)
                .Include(p => p.Replies)
                .FirstOrDefault();

            var postVMResults = _mapper.Map<PostViewModel>(post);

            return postVMResults;
        }

        public List<PostViewModel> GetAllPosts(int page, int pageSize)
        {
            // 計算跳過的資料量
            int skipAmount = (page - 1) * pageSize;

            var posts = _dbContext.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.PostDate)
                .Skip(skipAmount)
                .Take(pageSize)
                .ToList();

            var postVMResults = _mapper.Map<List<PostViewModel>>(posts);
            // 沒有用automapper的話，如下
            //List<PostViewModel> postVMResults = new List<PostViewModel>();
            //foreach (var post in posts)
            //{
            //    postVMResults.Add(new PostViewModel()
            //    {
            //        PostId = post.PostId,
            //        UserId = post.UserId,
            //        ...
            //    });
            //}
            return postVMResults;
        }

        public List<PostViewModel> GetMyPosts(int page, int pageSize)
        {
            // 計算跳過的資料量
            int skipAmount = (page - 1) * pageSize;

            var posts = new List<Post>();

            if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("Username")))
            {
                string username = _httpContextAccessor.HttpContext.Session.GetString("Username") ?? string.Empty;

                posts = _dbContext.Posts
                    .Where(p => p.User != null && p.User.Username == username)
                    .Include(p => p.User)
                    .OrderByDescending (p => p.PostDate)
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList();
            }

            var postVMResults = _mapper.Map<List<PostViewModel>>(posts);
            return postVMResults;
        }

        public void CreatePost(PostViewModel postVM)
        {
            string username = _httpContextAccessor.HttpContext?.Session.GetString("Username") ?? string.Empty;

            postVM.PostDate = DateTime.Now;
            postVM.LastEditDate = DateTime.Now;
            postVM.UserId = _dbContext.Users.Where(u => u.Username == username).Select(u => u.UserId).FirstOrDefault();

            var postMap = _mapper.Map<Post>(postVM);
            _dbContext.Posts.Add(postMap);
            _dbContext.SaveChanges();
        }

        public void DeletePost(PostViewModel postVM)
        {
            // 根據 PostId 查詢相應的貼文
            var post = _dbContext.Posts.FirstOrDefault(p => p.PostId == postVM.PostId);

            if (post != null)
            {
                // 如果找到了貼文，則從資料庫中刪除
                _dbContext.Posts.Remove(post);
                _dbContext.SaveChanges();
            }
        }

        public int GetTotalPostCount()
        {
            return _dbContext.Posts.Count();
        }
    }
}
