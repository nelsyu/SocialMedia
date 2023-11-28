using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class FriendshipStatus
{
    public int StatusId { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
}
