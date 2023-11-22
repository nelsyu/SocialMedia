using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicViewModel>> GetAllTopicsAsync();
        Task CreateTopicAsync(TopicViewModel topicVM, string title);
        Task DeleteTopicAsync(TopicViewModel topicVM);
    }
}
