﻿using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> ValidateRegisterAsync(UserViewModel userVM);
        Task RegisterAsync(UserViewModel userVM);
        Task<List<string>> ValidateLoginAsync(UserViewModel userVM);
        Task LoginSuccessfulAsync(int? userId);
        Task<bool> IsLoginAsync();
        Task LogoutAsync();
        Task DeleteAccountAsync(UserViewModel userVM);
        Task SaveQRCodeOTPAsync(string QRCodeOTPSK);
        Task<string> IsQRCodeOTPSecretKeyAsync(int? userId);
        Task<List<string>> VerifyQRCodeOTPAsync(int? userId, string confirmTotp);
        Task<(byte[], string)> GetOTPQRCodeAsync();
        Task<bool> FindRole(int? userId, int roleId);
        Task<int> FindUserId(string userVMEmail);
        Task<List<NotificationViewModel>> GetNotificationsAsync();
    }
}
