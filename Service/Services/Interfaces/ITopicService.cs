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
        List<TopicViewModel> GetAllTopics();
        void CreateTopic(TopicViewModel topicVM, string title);
        void DeleteTopic(TopicViewModel topicVM);
    }
}
