using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public int ReceiverUserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public int SenderUserId { get; set; }

    public virtual User ReceiverUser { get; set; } = null!;

    public virtual User SenderUser { get; set; } = null!;
}
