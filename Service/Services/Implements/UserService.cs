using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using Service.Services.Interfaces;
using Service.ViewModels;
using Library.Extensions;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Library.Constants;
using Lazy.Captcha.Core;

namespace Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISession? _session;
        private readonly ICaptcha _captcha;

        public UserService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICaptcha captcha)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _session = httpContextAccessor.HttpContext?.Session;
            _captcha = captcha;
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

        public async Task<List<string>> ValidateLoginAsync(UserViewModel userVM)
        {
            bool IsEmailInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email);
            bool IsPasswordInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email && u.Password == userVM.Password);
            bool IsConfirmCaptchaInvalid = !_captcha.Validate(userVM.UId, userVM.CaptchaCode);

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

        public async Task LoginSuccessfulAsync(int? userId)
        {
            UserLoggedIn? userLoggedIn = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserLoggedIn { UserId = u.Id, Username = u.Username, Email = u.Email })
                .FirstOrDefaultAsync();

            if (userLoggedIn != null)
                _session?.SetObject(ParameterKeys.UserLoggedIn, userLoggedIn);
        }

        public Task<bool> IsLoginAsync()
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            bool isLogin = !string.IsNullOrEmpty(sessionUserLoggedIn?.Username);
            return Task.FromResult(isLogin);
        }

        public Task LogoutAsync()
        {
            _session?.Remove(ParameterKeys.UserLoggedIn);
            return Task.CompletedTask;
        }

        public async Task DeleteAccountAsync(UserViewModel userVM)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            User? userEnt = null;

            if (sessionUserLoggedIn != null)
                userEnt = await _dbContext.Users
                    .Where(u => u.Id == sessionUserLoggedIn.UserId)
                    .FirstOrDefaultAsync();

            if (userEnt != null)
            {
                _dbContext.Users.Remove(userEnt);
                await _dbContext.SaveChangesAsync();
                _session?.Remove(ParameterKeys.UserLoggedIn);
            }
        }

        public async Task SaveQRCodeOTPAsync(string QRCodeOTPSK)
        {
            User? userEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
                userEnt = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == sessionUserLoggedIn.UserId);

            if (userEnt != null)
            {
                userEnt.Totp = QRCodeOTPSK;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<string> IsQRCodeOTPSecretKeyAsync(int? userId)
        {
            string qRCodeOTPSecretKey = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Totp)
                .FirstOrDefaultAsync() ?? "";

            return qRCodeOTPSecretKey;
        }

        public async Task<List<string>> VerifyQRCodeOTPAsync(int? userId, string confirmQRCodeOTP)
        {
            string? qRCodeOTPSecretKey = await _dbContext.Users
                .Where(u => u.Id == userId)
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

        public async Task<(byte[], string)> GetOTPQRCodeAsync()
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            string secretKey = Base32Encoding.ToString(OtpNet.KeyGeneration.GenerateRandomKey());

            string issuer = "SocialMedia";
            string label = sessionUserLoggedIn?.Username ?? "";
            string keyUri = new OtpUri(OtpType.Totp, secretKey, label, issuer).ToString();

            byte[] image = PngByteQRCodeHelper.GetQRCode(keyUri, QRCodeGenerator.ECCLevel.Q, 10);

            await Task.CompletedTask;

            return (image, secretKey);
        }

        public async Task<bool> FindRole(int? userId, int roleId)
        {
            var userEnt = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();

            if (userEnt == null)
            {
                return false;
            }

            bool isRoleExists = userEnt.Roles.Any(r => r.Id == roleId);

            return isRoleExists;
        }

        public async Task<int> FindUserId(string userVMEmail)
        {
            int userIdEnt = await _dbContext.Users
                .Where(u => u.Email == userVMEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            return userIdEnt;
        }

        public async Task<List<NotificationViewModel>> GetNotificationsAsync()
        {
            List<Notification>? notificationsEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if(sessionUserLoggedIn != null)
            {
                notificationsEnt = await _dbContext.Notifications
                    .Where(n => n.ReceiverUserId == sessionUserLoggedIn.UserId)
                    .OrderByDescending(n => n.CreateTime)
                    .ToListAsync();
            }
            List<NotificationViewModel> notificationsVM = _mapper.Map<List<NotificationViewModel>>(notificationsEnt);
            
            return notificationsVM;
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
