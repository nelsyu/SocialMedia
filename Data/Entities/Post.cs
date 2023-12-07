using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Post
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime EditDate { get; set; }

    public int TopicId { get; set; }

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual Topic Topic { get; set; } = null!;

    public virtual User? User { get; set; }
}
