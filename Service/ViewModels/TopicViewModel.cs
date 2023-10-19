﻿using System;
using System.Collections.Generic;

namespace Service.ViewModels
{
    public partial class TopicViewModel
    {
        public int TopicId { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = null!;

        public virtual UserViewModel? UserViewModel { get; set; }
    }
}
