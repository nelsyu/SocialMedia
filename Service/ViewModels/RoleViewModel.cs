namespace Service.ViewModels;

public partial class RoleViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserViewModel> Users { get; set; } = new List<UserViewModel>();
}
