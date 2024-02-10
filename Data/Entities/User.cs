using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Totp { get; set; }

    public virtual ICollection<Friendship> FriendshipUserId1Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUserId2Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
