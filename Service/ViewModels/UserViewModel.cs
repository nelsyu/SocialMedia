using Data.Entities;

namespace Service.ViewModels
{
    public partial class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Totp { get; set; }

        public virtual ICollection<FriendshipViewModel> FriendshipUserId1Navigations { get; set; } = new List<FriendshipViewModel>();
        public virtual ICollection<FriendshipViewModel> FriendshipUserId2Navigations { get; set; } = new List<FriendshipViewModel>();
        public virtual ICollection<LikeViewModel> Likes { get; set; } = new HashSet<LikeViewModel>();
        public virtual ICollection<NotificationViewModel> Notifications { get; set; } = new List<NotificationViewModel>();
        public virtual ICollection<PostViewModel> Posts { get; set; } = new HashSet<PostViewModel>();
        public virtual ICollection<ReplyViewModel> Replies { get; set; } = new HashSet<ReplyViewModel>();
        public virtual ICollection<TopicViewModel> Topics { get; set; } = new HashSet<TopicViewModel>();
        public virtual ICollection<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        #region My property
        public string ConfirmPassword { get; set; } = null!;
        public string UId { get; set; } = null!;
        public string CaptchaCode { get; set; } = null!;
        #endregion
    }
}
