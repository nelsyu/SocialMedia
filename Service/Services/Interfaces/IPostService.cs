using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IPostService
    {
        PostViewModel GetPost(int postId);
        List<PostViewModel> GetAllPosts(int page, int pageSize);
        List<PostViewModel> GetMyPosts(int page, int pageSize);
        void CreatePost(PostViewModel postVM);
        void DeletePost(PostViewModel postVM);
        int GetTotalPostCount();
    }
}
