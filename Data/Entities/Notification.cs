using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedTime { get; set; }

    public virtual User User { get; set; } = null!;
}
