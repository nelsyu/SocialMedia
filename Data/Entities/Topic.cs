using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class Topic
    {
        public Topic()
        {
            Posts = new HashSet<Post>();
        }

        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Post> Posts { get; set; }
    }
}
