using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using Service.Services.Interfaces;
using Service.ViewModels;
using Service.Extensions;
using Library.Extensions;
using System.Text.RegularExpressions;

namespace Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserLoggedIn _userLoggedIn;

        public UserService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserLoggedIn userLoggedIn)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userLoggedIn = userLoggedIn;
        }

        public async Task<List<string>> ValidateRegisterAsync(UserViewModel userVM)
        {
            string IsEmailValidate = IsValidEmailAsync(userVM.Email);
            bool IsUsernameInvalid = await _dbContext.Users.AnyAsync(u => u.Username == userVM.Username) || userVM.Username == null;
            bool IsPasswordInvalid = string.IsNullOrEmpty(userVM.Password);
            bool IsConfirmPasswordInvalid = string.IsNullOrEmpty(userVM.ConfirmPassword) || userVM.Password != userVM.ConfirmPassword;
            List<string> result = new();

            if (IsEmailValidate != "true")
            {
                result.Add("Email");
                result.Add(IsEmailValidate);
            }
            else if (IsUsernameInvalid)
            {
                result.Add("Username");
                result.Add("Username is invalid.");
            }
            else if (IsPasswordInvalid)
            {
                result.Add("Password");
                result.Add("Password is invalid.");
            }
            else if (IsConfirmPasswordInvalid)
            {
                result.Add("ConfirmPassword");
                result.Add("ConfirmPassword is invalid.");
            }
            else
                result.Add("");

            return result;
        }

        public async Task RegisterAsync(UserViewModel userVM)
        {
            User userEnt = _mapper.Map<User>(userVM);
            _dbContext.Users.Add(userEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<string>> ValidateLoginAsync(UserViewModel userVM, object captchaCode)
        {
            bool IsEmailInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email);
            bool IsPasswordInvalid = !await _dbContext.Users.AnyAsync(u => u.Password == userVM.Password);
            bool IsConfirmCaptchaInvalid = !Equals(captchaCode, userVM.ConfirmCaptcha);

            List<string> result = new();

            if (IsEmailInvalid)
            {
                result.Add("Email");
                result.Add("Email is invalid.");
            }
            else if (IsPasswordInvalid)
            {
                result.Add("Password");
                result.Add("Password is invalid.");
            }
            else if (IsConfirmCaptchaInvalid)
            {
                result.Add("Captcha");
                result.Add("Captcha is invalid");
            }
            else
            {
                result.Add("");
            }

            return result;
        }

        public async Task LoginSuccessfulAsync(string UserVMEmail)
        {
            User? userEnt = await _dbContext.Users.Where(u => u.Email == UserVMEmail).FirstOrDefaultAsync();
            if (userEnt != null)
            {
                UserViewModel userVM = _mapper.Map<UserViewModel>(userEnt);
                _userLoggedIn.Set(userVM);
            }
        }

        public Task<bool> IsLoginAsync()
        {
            bool isLogin = !string.IsNullOrEmpty(_userLoggedIn.Username);
            return Task.FromResult(isLogin);
        }

        public Task LogoutAsync()
        {
            _userLoggedIn.Dispose();
            return Task.CompletedTask;
        }

        public async Task DeleteAccountAsync(UserViewModel userVM)
        {
            User? userEnt = await _dbContext.Users.Where(u => u.Username == _userLoggedIn.Username).FirstOrDefaultAsync();

            if (userEnt != null)
            {
                _dbContext.Users.Remove(userEnt);
                await _dbContext.SaveChangesAsync();
                _userLoggedIn.Dispose();
            }
        }

        public async Task SaveQRCodeOTPAsync(string QRCodeOTPSK)
        {
            User? userEnt = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == _userLoggedIn.Username);
            if (userEnt != null)
            {
                userEnt.Totp = QRCodeOTPSK;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<string> IsQRCodeOTPSecretKeyAsync(string UserVMEmail)
        {
            string qRCodeOTPSecretKey = await _dbContext.Users
                .Where(u => u.Email == UserVMEmail)
                .Select(u => u.Totp)
                .FirstOrDefaultAsync() ?? "";

            return qRCodeOTPSecretKey;
        }

        public async Task<List<string>> VerifyQRCodeOTPAsync(string userVMEmail, string confirmQRCodeOTP)
        {
            string? qRCodeOTPSecretKey = await _dbContext.Users
                .Where(u => u.Email == userVMEmail)
                .Select(u => u.Totp)
                .FirstOrDefaultAsync();

            Totp qRCodeOTP = new(Base32Encoding.ToBytes((string)(qRCodeOTPSecretKey ?? "")));
            bool isConfirmQRCodeOTPInvalid = !qRCodeOTP.VerifyTotp(confirmQRCodeOTP, out _);

            List<string> result = new();

            if (isConfirmQRCodeOTPInvalid)
            {
                result.Add("ConfirmQRCodeOTP");
                result.Add("ConfirmQRCodeOTP is invalid");
            }
            else
            {
                result.Add("");
            }

            return result;
        }

        public async Task<(byte[], string)> GenerateCaptchaImageAsync()
        {
            var validateCodeProvider = new SkiaSharpValidateCodeHelper
            {
                SetWith = 100,
                SetHeight = 40,
                SetFontSize = 20
            };

            byte[] img = validateCodeProvider.GetVerifyCodeImage();
            string captchaCode = validateCodeProvider.SetVerifyCodeText;

            await Task.CompletedTask;

            return (img, captchaCode);
        }

        public async Task<(byte[], string)> GenerateOTPQRCodeAsync()
        {
            string secretKey = Base32Encoding.ToString(OtpNet.KeyGeneration.GenerateRandomKey());

            string issuer = "SocialMedia";
            string label = _userLoggedIn.Username ?? "";
            string keyUri = new OtpUri(OtpType.Totp, secretKey, label, issuer).ToString();

            byte[] image = PngByteQRCodeHelper.GetQRCode(keyUri, QRCodeGenerator.ECCLevel.Q, 10);

            await Task.CompletedTask;

            return (image, secretKey);
        }

        public async Task<List<FriendshipViewModel>> GetAllFriendsAsync()
        {
            List<Friendship> friendshipEntL = await _dbContext.Friendships
                .Where(f => f.UserId1 == _userLoggedIn.UserId)
                .ToListAsync();

            List<FriendshipViewModel> friendshipsVM = _mapper.Map<List<FriendshipViewModel>>(friendshipEntL);

            foreach (var friendshipVM in friendshipsVM)
            {
                friendshipVM.User2 = _mapper.Map<UserViewModel>(await _dbContext.Users.FindAsync(friendshipVM.UserId2));
            }

            return friendshipsVM;
        }

        #region private methods
        private string IsValidEmailAsync(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrWhiteSpace(email))
            {
                return "電子郵件地址不能為空。";
            }

            if (_dbContext.Users.Any(u => u.Email == email))
            {
                return "電子郵件地址已被註冊。";
            }

            int atIndex = email.IndexOf('@');
            if (atIndex == -1)
            {
                return "電子郵件地址缺少 '@' 符號。";
            }

            int dotIndex = email.LastIndexOf('.');
            if (dotIndex == -1 || dotIndex < atIndex)
            {
                return "電子郵件地址缺少點（.），或點（.）的位置不正確。";
            }

            if (!Regex.IsMatch(email, emailPattern))
            {
                return "電子郵件地址格式無效。";
            }

            return "true";
        }
        #endregion
    }
}
