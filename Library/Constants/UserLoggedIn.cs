using System.Data;

namespace Library.Constants
{
    public class UserLoggedIn
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public ICollection<int>? RolesId { get; set; }
    }
}
