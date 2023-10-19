using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class Topic
    {
        public int TopicId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = null!;

        public virtual User? User { get; set; }
    }
}
