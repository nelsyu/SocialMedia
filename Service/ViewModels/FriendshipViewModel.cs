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
        public string? Status { get; set; }
        public DateTime? CreatedTime { get; set; }

        #region My Property
        public virtual UserViewModel User2 { get; set; }
        #endregion My Property
    }
}
