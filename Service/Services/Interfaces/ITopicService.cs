using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicViewModel>> GetAllTopicsAsync();
        Task CreateTopicAsync(TopicViewModel topicVM, string title);
        Task DeleteTopicAsync(TopicViewModel topicVM);
    }
}
