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

namespace Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISession? _session;

        public UserService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _session = httpContextAccessor.HttpContext?.Session;
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
            bool IsPasswordInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email && u.Password == userVM.Password);
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

        public async Task<List<UserViewModel>> GetAllFriendsAsync()
        {
            List<Friendship>? friendshipsEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
            {
                friendshipsEnt = await _dbContext.Friendships
                    .Where(f => (f.FriendshipStatusId == 1) && ((f.UserId1 == sessionUserLoggedIn.UserId) || (f.UserId2 == sessionUserLoggedIn.UserId)))
                    .ToListAsync();
            }

            List<FriendshipViewModel> friendshipsVM = _mapper.Map<List<FriendshipViewModel>>(friendshipsEnt);
            List<UserViewModel> usersVM = new List<UserViewModel>();

            foreach (var friendshipVM in friendshipsVM)
            {
                int? userId2 = friendshipVM.UserId1 == sessionUserLoggedIn?.UserId ? friendshipVM.UserId2 : friendshipVM.UserId1;
                User? userEnt = await _dbContext.Users.FindAsync(userId2);
                UserViewModel userVM = _mapper.Map<UserViewModel>(userEnt);
                usersVM.Add(userVM);
            }

            return usersVM;
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

        public async Task FriendAdd(int userId2)
        {
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);
            FriendshipViewModel friendShipVM = new FriendshipViewModel
            {
                UserId1 = sessionUserLoggedIn?.UserId,
                UserId2 = userId2,
                FriendshipStatusId = 2,
                CreateTime = DateTime.Now
            };

            Friendship friendshipEnt = _mapper.Map<Friendship>(friendShipVM);
            _dbContext.Friendships.Add(friendshipEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task FriendConfirm(int userId2)
        {
            Friendship? friendshipEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
                friendshipEnt = await _dbContext.Friendships.Where(f => (f.UserId1 == userId2 && f.UserId2 == sessionUserLoggedIn.UserId)).FirstOrDefaultAsync();
            if (friendshipEnt != null)
            {
                friendshipEnt.FriendshipStatusId = 1;
                _dbContext.Friendships.Update(friendshipEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task FriendDeny(int userId2)
        {
            Friendship? friendshipEnt = null;
            Notification? notificationEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
            {
                friendshipEnt = await _dbContext.Friendships
                    .Where(f => (f.UserId1 == sessionUserLoggedIn.UserId && f.UserId2 == userId2) || (f.UserId1 == userId2 && f.UserId2 == sessionUserLoggedIn.UserId))
                    .FirstOrDefaultAsync();

                if(friendshipEnt != null)
                {
                    _dbContext.Friendships.Remove(friendshipEnt);
                    await _dbContext.SaveChangesAsync();
                }

                notificationEnt = await _dbContext.Notifications
                    .Where(n => n.ReceiverUserId == userId2 && n.Message == sessionUserLoggedIn.Username + " want to be your friend!")
                    .FirstOrDefaultAsync();

                if (notificationEnt != null)
                {
                    _dbContext.Notifications.Remove(notificationEnt);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<int?> FriendshipStatus(int userId2)
        {
            Friendship? friendshipEnt = null;
            UserLoggedIn? sessionUserLoggedIn = _session?.GetObject<UserLoggedIn>(ParameterKeys.UserLoggedIn);

            if (sessionUserLoggedIn != null)
                friendshipEnt = await _dbContext.Friendships
                    .FirstOrDefaultAsync(f => (f.UserId1 == sessionUserLoggedIn.UserId && f.UserId2 == userId2));

            if (friendshipEnt != null)
                return friendshipEnt.FriendshipStatusId;

            if (sessionUserLoggedIn != null)
                friendshipEnt = await _dbContext.Friendships
                    .FirstOrDefaultAsync(f => (f.UserId1 == userId2 && f.UserId2 == sessionUserLoggedIn.UserId));

            if (friendshipEnt != null)
            {
                if(friendshipEnt.FriendshipStatusId == 2)
                    return friendshipEnt.FriendshipStatusId + 1;
                else
                    return friendshipEnt.FriendshipStatusId;
            }

            return null;
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
