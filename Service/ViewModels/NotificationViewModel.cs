using Data.Entities;
using System;
using System.Collections.Generic;

namespace Service.ViewModels;

public partial class NotificationViewModel
{
    public int Id { get; set; }

    public int ReceiverUserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public int SenderUserId { get; set; }

    public virtual User ReceiverUser { get; set; } = null!;
}
