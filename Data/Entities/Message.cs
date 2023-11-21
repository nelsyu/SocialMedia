using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Message
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentTime { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsArchived { get; set; }
}
