using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class PostViewModel
    {
        public PostViewModel()
        {
            ReplyViewModels = new HashSet<ReplyViewModel>();
            TopicViewModels = new HashSet<TopicViewModel>();
        }

        public int PostId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public DateTime? LastEditDate { get; set; }
        public int TopicId { get; set; }

        public virtual TopicViewModel TopicViewModel { get; set; } = null!;
        public virtual UserViewModel? UserViewModel { get; set; }
        public virtual ICollection<ReplyViewModel> ReplyViewModels { get; set; }

        // My Property
        public ICollection<TopicViewModel> TopicViewModels { get; set; }
    }
}
