using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class User
    {
        public User()
        {
            Likes = new HashSet<Like>();
            Posts = new HashSet<Post>();
            Replies = new HashSet<Reply>();
            Topics = new HashSet<Topic>();
        }

        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Totp { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
