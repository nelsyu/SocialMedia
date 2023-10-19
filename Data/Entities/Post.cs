﻿using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class Post
    {
        public Post()
        {
            Replies = new HashSet<Reply>();
        }

        public int PostId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public DateTime? LastEditDate { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
    }
}
