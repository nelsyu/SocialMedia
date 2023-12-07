﻿using Data.Entities;

namespace Service.ViewModels
{
    public partial class UserViewModel
    {
        public UserViewModel()
        {
            Likes = new HashSet<LikeViewModel>();
            Posts = new HashSet<PostViewModel>();
            Replies = new HashSet<ReplyViewModel>();
            Topics = new HashSet<TopicViewModel>();
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Totp { get; set; }

        #region My property
        public string ConfirmPassword { get; set; } = null!;
        public byte[] CaptchaImage { get; set; } = null!;
        public string ConfirmCaptcha { get; set; } = null!;
        public string ConfirmQRCodeOTP { get; set; } = null!;
        #endregion

        public virtual ICollection<FriendshipViewModel> FriendshipUserId1Navigations { get; set; } = new List<FriendshipViewModel>();
        public virtual ICollection<FriendshipViewModel> FriendshipUserId2Navigations { get; set; } = new List<FriendshipViewModel>();
        public virtual ICollection<LikeViewModel> Likes { get; set; }
        public virtual ICollection<NotificationViewModel> NotificationSourceUsers { get; set; } = new List<NotificationViewModel>();
        public virtual ICollection<NotificationViewModel> NotificationUsers { get; set; } = new List<NotificationViewModel>();
        public virtual ICollection<PostViewModel> Posts { get; set; }
        public virtual ICollection<ReplyViewModel> Replies { get; set; }
        public virtual ICollection<TopicViewModel> Topics { get; set; }
        public virtual ICollection<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }
}
