using Service.ViewModels;

namespace Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> ValidateRegisterAsync(UserViewModel userVM);
        Task RegisterAsync(UserViewModel userVM);
        Task<List<string>> ValidateLoginAsync(UserViewModel userVM, object captchaCode);
        Task LoginSuccessfulAsync(int? userId);
        Task<bool> IsLoginAsync();
        Task LogoutAsync();
        Task DeleteAccountAsync(UserViewModel userVM);
        Task SaveQRCodeOTPAsync(string QRCodeOTPSK);
        Task<string> IsQRCodeOTPSecretKeyAsync(int? userId);
        Task<List<string>> VerifyQRCodeOTPAsync(int? userId, string confirmTotp);
        Task<(byte[], string)> GenerateCaptchaImageAsync();
        Task<(byte[], string)> GenerateOTPQRCodeAsync();
        Task<List<UserViewModel>> GetAllFriendsAsync();
        Task<bool> FindRole(int? userId, int roleId);
        Task<int> FindUserId(string userVMEmail);
        Task FriendAdd(int userId2);
        Task FriendConfirm(int userId2);
        Task FriendDeny(int userId2);
        Task<int?> FriendshipStatus(int userId2);
        Task<List<NotificationViewModel>> GetNotificationsAsync();
    }
}
