using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Friendship
{
    public int Id { get; set; }

    public int UserId1 { get; set; }

    public int UserId2 { get; set; }

    public int Status { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual FriendshipStatus StatusNavigation { get; set; } = null!;

    public virtual User UserId1Navigation { get; set; } = null!;

    public virtual User UserId2Navigation { get; set; } = null!;
}
