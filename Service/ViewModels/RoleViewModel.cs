namespace Service.ViewModels;

public partial class RoleViewModel
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserViewModel> Users { get; set; } = new List<UserViewModel>();
}
