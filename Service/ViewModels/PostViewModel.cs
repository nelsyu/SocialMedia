namespace Service.ViewModels
{
    public partial class PostViewModel
    {
        public PostViewModel()
        {
            Likes = new HashSet<LikeViewModel>();
            Replies = new HashSet<ReplyViewModel>();
            Topics = new HashSet<TopicViewModel>();
        }

        public int PostId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public DateTime? LastEditDate { get; set; }
        public int TopicId { get; set; }

        public virtual TopicViewModel Topic { get; set; } = null!;
        public virtual UserViewModel? User { get; set; }
        public virtual ICollection<LikeViewModel> Likes { get; set; }
        public virtual ICollection<ReplyViewModel> Replies { get; set; }

        #region My Property
        public ICollection<TopicViewModel> Topics { get; set; }
        #endregion My Property
    }
}
