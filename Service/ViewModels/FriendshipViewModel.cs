﻿using Data.Entities;

namespace Service.ViewModels
{
    public partial class FriendshipViewModel
    {
        public FriendshipViewModel()
        {
            User2 = new UserViewModel();        
        }

        public int Id { get; set; }
        public int? UserId1 { get; set; }
        public int? UserId2 { get; set; }
        public int? FriendshipStatusId { get; set; }
        public DateTime? CreateTime { get; set; }
        public virtual FriendshipStatusViewModel FriendshipStatus { get; set; } = null!;
        public virtual UserViewModel UserId1Navigation { get; set; } = null!;
        public virtual UserViewModel UserId2Navigation { get; set; } = null!;

        #region My Property
        public virtual UserViewModel User2 { get; set; }
        #endregion My Property
    }
}
