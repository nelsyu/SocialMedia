using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class LikeViewModel
    {
        public int LikeId { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }
        public int? ReplyId { get; set; }
        public string? EmojiSymbol { get; set; }

        public virtual PostViewModel? PostViewModel { get; set; }
        public virtual ReplyViewModel? ReplyViewModel { get; set; }
        public virtual UserViewModel? UserViewModel { get; set; }
    }
}
