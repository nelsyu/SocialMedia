using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class FriendshipStatus
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
}
