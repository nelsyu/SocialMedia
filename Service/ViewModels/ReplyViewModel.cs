using Data.Entities;
using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class ReplyViewModel
    {
        public ReplyViewModel()
        {
            LikeViewModels = new HashSet<LikeViewModel>();
        }

        public int ReplyId { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime ReplyDate { get; set; }

        public virtual PostViewModel? PostViewModel { get; set; }
        public virtual UserViewModel? UserViewModel { get; set; }
        public virtual ICollection<LikeViewModel> LikeViewModels { get; set; }
    }
}
