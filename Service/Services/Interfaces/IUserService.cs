using Data.Entities;
using Microsoft.AspNetCore.Http;
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
        List<string> ValidateRegister(UserViewModel userVM);
        void Register(UserViewModel userVM);
        List<string> ValidateLogin(UserViewModel userVM, object captchaCode);
        void LoginSuccessful(string userVMEmail);
        bool IsLogin();
        void Logout();
        void DeleteAccount(UserViewModel userVM);
        void SaveQRCodeOTP(string QRCodeOTPSK);
        string ?IsQRCodeOTPSecretKey(string userVMEmail);
        List<string> VerifyQRCodeOTP(string userVMEmail, string confirmTotp);
        byte[] GenerateCaptchaImage(out string captchaCode);
        byte[] GenerateOTPQRCode(out string secretKey);
    }
}
