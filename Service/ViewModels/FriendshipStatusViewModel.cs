namespace Service.ViewModels;

public partial class FriendshipStatusViewModel
{
    public int StatusId { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<FriendshipViewModel> Friendships { get; set; } = new List<FriendshipViewModel>();
}
