using Data.Entities;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Extensions
{
    public class UserLoggedIn
    {
        public int UserId;
        public string? Email;
        public string? Username;
        public string? Password;
        public string? QRCodeOTPSK;

        public void Set(UserViewModel userVM)
        {
            UserId = userVM.UserId;
            Email = userVM.Email;
            Username = userVM.Username;
            Password = userVM.Password;
            QRCodeOTPSK = userVM.Totp;
        }

        public void Dispose()
        {
            UserId = 0;
            Email = null;
            Username = null;
            Password = null;
            QRCodeOTPSK = null;
        }
    }
}
