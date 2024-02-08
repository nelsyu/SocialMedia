using Library.Models;
using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(UserViewModel userVM);
        Task DeleteAccountAsync(UserViewModel userVM);
        Task SaveQRCodeOTPAsync(string QRCodeOTPSK);
        Task<bool> HasQRCodeOTPSKAsync(int userId);
        Task<List<string>> VerifyQRCodeOTPAsync(int userId, string myQRCodeOTP);
        Task<(byte[], string)> GetOTPQRCodeAsync();
        Task<int> FindUserIdAsync(string userInfo);
        Task<List<NotificationViewModel>> GetNotificationAsync();
        Task<UserLoggedIn?> GetUserInfoAsync(int userId);
    }
}
