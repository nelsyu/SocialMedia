using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.ViewModels
{
    public partial class UserViewModel
    {
        public UserViewModel()
        {
            PostsViewModels = new HashSet<PostViewModel>();
            RepliesViewModels = new HashSet<ReplyViewModel>();
            TopicsViewModels = new HashSet<TopicViewModel>();
        }

        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Totp { get; set; }

        #region My property
        public string ConfirmPassword { get; set; } = null!;
        public string ConfirmCaptcha { get; set; } = null!;
        public string ConfirmQRCodeOTP { get; set; } = null!;
        #endregion

        public virtual ICollection<PostViewModel> PostsViewModels { get; set; }
        public virtual ICollection<ReplyViewModel> RepliesViewModels { get; set; }
        public virtual ICollection<TopicViewModel> TopicsViewModels { get; set; }
    }
}
