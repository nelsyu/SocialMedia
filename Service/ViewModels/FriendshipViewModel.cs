using Data.Entities;

namespace Service.ViewModels
{
    public partial class FriendshipViewModel
    {
        public FriendshipViewModel()
        {
            User2 = new UserViewModel();        
        }

        public int FriendshipId { get; set; }
        public int? UserId1 { get; set; }
        public int? UserId2 { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public virtual FriendshipStatusViewModel StatusNavigation { get; set; } = null!;
        public virtual UserViewModel UserId1Navigation { get; set; } = null!;
        public virtual UserViewModel UserId2Navigation { get; set; } = null!;

        #region My Property
        public virtual UserViewModel User2 { get; set; }
        #endregion My Property
    }
}
