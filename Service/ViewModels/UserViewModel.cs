using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.ViewModels
{
    public partial class UserViewModel
    {
        public UserViewModel()
        {
            PostsViewModels = new HashSet<PostViewModel>();
            RepliesViewModels = new HashSet<ReplyViewModel>();
            TopicsViewModels = new HashSet<TopicViewModel>();
        }

        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;

        public virtual ICollection<PostViewModel> PostsViewModels { get; set; }
        public virtual ICollection<ReplyViewModel> RepliesViewModels { get; set; }
        public virtual ICollection<TopicViewModel> TopicsViewModels { get; set; }
    }
}
