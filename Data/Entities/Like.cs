using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Like
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PostId { get; set; }

    public int? ReplyId { get; set; }

    public string? EmojiSymbol { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Reply? Reply { get; set; }

    public virtual User? User { get; set; }
}
