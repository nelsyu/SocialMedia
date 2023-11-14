using Data.Entities;
using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class ReplyViewModel
    {
        public ReplyViewModel()
        {
            Likes = new HashSet<LikeViewModel>();
        }

        public int ReplyId { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime ReplyDate { get; set; }

        public virtual PostViewModel? Post { get; set; }
        public virtual UserViewModel? User { get; set; }
        public virtual ICollection<LikeViewModel> Likes { get; set; }
    }
}
