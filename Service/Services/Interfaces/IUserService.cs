using Service.ViewModels;

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
