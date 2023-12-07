namespace Service.ViewModels
{
    public partial class ReplyViewModel
    {
        public ReplyViewModel()
        {
            Likes = new HashSet<LikeViewModel>();
        }

        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        public virtual PostViewModel? Post { get; set; }
        public virtual UserViewModel? User { get; set; }
        public virtual ICollection<LikeViewModel> Likes { get; set; }
    }
}
