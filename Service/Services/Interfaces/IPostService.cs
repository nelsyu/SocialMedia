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
        List<PostViewModel> GetAllPosts();
        List<PostViewModel> GetAllPosts(UserViewModel userNowVM);
        List<PostViewModel> GetAllPosts(int topicId);
        List<PostViewModel> Paging(List<PostViewModel> postVMs, int page, int pageSize);
        void CreatePost(PostViewModel postVM);
        void UpdatePost(PostViewModel postVM, int postId);
        void DeletePost(PostViewModel postVM);
    }
}
