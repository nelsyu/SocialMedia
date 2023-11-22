namespace Service.ViewModels
{
    public partial class TopicViewModel
    {
        public TopicViewModel()
        {
            Posts = new HashSet<PostViewModel>();
        }

        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;

        public virtual UserViewModel User { get; set; } = null!;
        public virtual ICollection<PostViewModel> Posts { get; set; }
    }
}
