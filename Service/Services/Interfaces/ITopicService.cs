using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicViewModel>> GetAllTopicsAsync();
        Task CreateTopicAsync(string title);
        Task<bool> DeleteTopicAsync(int topicId);
    }
}
