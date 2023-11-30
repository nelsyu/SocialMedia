using Service.ViewModels;

namespace Service.Extensions
{
    public class UserLoggedIn
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
