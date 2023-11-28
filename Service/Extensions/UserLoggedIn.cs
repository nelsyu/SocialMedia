using Service.ViewModels;

namespace Service.Extensions
{
    public class UserLoggedIn
    {
        public int UserId;
        public string? Email;
        public string? Username;

        public void Set(UserViewModel userVM)
        {
            UserId = userVM.UserId;
            Email = userVM.Email;
            Username = userVM.Username;
        }

        public void Dispose()
        {
            UserId = 0;
            Email = null;
            Username = null;
        }
    }
}
