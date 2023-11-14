using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class Reply
    {
        public Reply()
        {
            Likes = new HashSet<Like>();
        }

        public int ReplyId { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime ReplyDate { get; set; }

        public virtual Post? Post { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
