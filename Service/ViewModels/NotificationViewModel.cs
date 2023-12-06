using Data.Entities;
using System;
using System.Collections.Generic;

namespace Service.ViewModels;

public partial class NotificationViewModel
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedTime { get; set; }

    public int SourceUserId { get; set; }

    public virtual UserViewModel SourceUser { get; set; } = null!;

    public virtual UserViewModel User { get; set; } = null!;
}
