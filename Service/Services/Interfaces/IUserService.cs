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
        Task<List<string>> ValidateRegisterAsync(UserViewModel userVM);
        Task RegisterAsync(UserViewModel userVM);
        Task<List<string>> ValidateLoginAsync(UserViewModel userVM, object captchaCode);
        Task LoginSuccessfulAsync(string userVMEmail);
        Task<bool> IsLoginAsync();
        Task LogoutAsync();
        Task DeleteAccountAsync(UserViewModel userVM);
        Task SaveQRCodeOTPAsync(string QRCodeOTPSK);
        Task<string> IsQRCodeOTPSecretKeyAsync(string userVMEmail);
        Task<List<string>> VerifyQRCodeOTPAsync(string userVMEmail, string confirmTotp);
        Task<(byte[], string)> GenerateCaptchaImageAsync();
        Task<(byte[], string)> GenerateOTPQRCodeAsync();
        Task<List<FriendshipViewModel>> GetAllFriendsAsync();
    }
}
