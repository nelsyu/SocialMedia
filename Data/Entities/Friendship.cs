using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Friendship
{
    public int FriendshipId { get; set; }

    public int UserId1 { get; set; }

    public int UserId2 { get; set; }

    public int Status { get; set; }

    public DateTime CreatedTime { get; set; }

    public virtual FriendshipStatus StatusNavigation { get; set; } = null!;
}
