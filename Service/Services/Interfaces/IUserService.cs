using Data.Entities;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IUserService
    {
        List<string> ValidateRegister(string email, string Username);
        void Register(UserViewModel userVM);
        List<string> ValidateLogin(string email, string password);
        bool IsLogin();
        void Logout();
    }
}
