using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostViewModel> GetPostAsync(int postId);
        Task<List<PostViewModel>> GetAllPostsAsync();
        Task<List<PostViewModel>> GetAllPostsAsync(int topicId);
        Task<List<PostViewModel>> GetMyPostsAsync();
        Task<List<PostViewModel>> GetMyPostsAsync(int userId);
        Task<List<PostViewModel>> PagingAsync(List<PostViewModel> postVMs, int page, int pageSize);
        Task CreatePostAsync(PostViewModel postVM);
        Task DeletePostAsync(int postId);
        Task UpdatePostAsync(PostViewModel postVM, int postId);
    }
}
