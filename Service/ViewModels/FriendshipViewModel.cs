using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class FriendshipViewModel
    {
        public int FriendshipId { get; set; }
        public int? UserId1 { get; set; }
        public int? UserId2 { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
