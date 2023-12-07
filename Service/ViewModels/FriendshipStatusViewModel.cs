namespace Service.ViewModels;

public partial class FriendshipStatusViewModel
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<FriendshipViewModel> Friendships { get; set; } = new List<FriendshipViewModel>();
}
