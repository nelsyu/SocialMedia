namespace Service.ViewModels
{
    public partial class LikeViewModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }
        public int? ReplyId { get; set; }
        public string? EmojiSymbol { get; set; }

        public virtual PostViewModel? Post { get; set; }
        public virtual ReplyViewModel? Reply { get; set; }
        public virtual UserViewModel? User { get; set; }
    }
}
