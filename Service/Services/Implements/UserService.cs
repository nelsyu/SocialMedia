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
using Library.Models;
using System.Security.Claims;

namespace Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public UserService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task RegisterAsync(UserViewModel userVM)
        {
            User userEnt = _mapper.Map<User>(userVM);
            _dbContext.Users.Add(userEnt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(UserViewModel userVM)
        {
            User? userEnt = await _dbContext.Users
                .Where(u => u.Email == _httpContext.User.Identity!.Name)
                .FirstOrDefaultAsync();

            if (userEnt != null)
            {
                _dbContext.Users.Remove(userEnt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SaveQRCodeOTPAsync(string QRCodeOTPSK)
        {
            User? userEnt = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == _httpContext.User.Identity!.Name);

            if (userEnt != null)
            {
                userEnt.Totp = QRCodeOTPSK;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> HasQRCodeOTPSKAsync(int userId)
        {
            bool hasQRCodeOTPSK = !string.IsNullOrEmpty(
                await _dbContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.Totp)
                    .FirstOrDefaultAsync()
                );

            return hasQRCodeOTPSK;
        }

        public async Task<List<string>> VerifyQRCodeOTPAsync(int userId, string QRCodeOTP)
        {
            string? qRCodeOTPSK = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Totp)
                .FirstOrDefaultAsync();

            Totp tOTP = new(Base32Encoding.ToBytes((string)(qRCodeOTPSK ?? "")));
            bool isQRCodeOTPInvalid = string.IsNullOrEmpty(QRCodeOTP) || !tOTP.VerifyTotp(QRCodeOTP, out _);

            List<string> result = new();

            if (isQRCodeOTPInvalid)
            {
                result.Add("QRCodeOTP");
                result.Add("QRCodeOTP is invalid");
            }
            else
            {
                result.Add("");
            }

            return result;
        }

        public async Task<(byte[], string)> GetOTPQRCodeAsync()
        {
            string secretKey = Base32Encoding.ToString(OtpNet.KeyGeneration.GenerateRandomKey());

            string issuer = "SocialMedia";
            string label = _httpContext.User.FindFirst(ParameterKeys.UsernameLoggedIn)!.Value;
            string keyUri = new OtpUri(OtpType.Totp, secretKey, label, issuer).ToString();

            byte[] image = PngByteQRCodeHelper.GetQRCode(keyUri, QRCodeGenerator.ECCLevel.Q, 10);

            await Task.CompletedTask;

            return (image, secretKey);
        }

        public async Task<int> FindUserIdAsync(string userInfo)
        {
            int userIdEnt = 0;

            if (userInfo.Contains('@'))
            {
                userIdEnt = await _dbContext.Users
                    .Where(u => u.Email == userInfo)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }
            else
            {
                userIdEnt = await _dbContext.Users
                    .Where(u => u.Username == userInfo)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            return userIdEnt;
        }

        public async Task<List<NotificationViewModel>> GetNotificationAsync()
        {
            List<Notification>? notificationsEnt  = await _dbContext.Notifications
                .Where(n => n.ReceiverUserId == Convert.ToInt32(_httpContext.User.FindFirstValue(ParameterKeys.UserIdLoggedIn)!))
                .OrderByDescending(n => n.CreateTime)
                .ToListAsync();

            List<NotificationViewModel> notificationsVM = _mapper.Map<List<NotificationViewModel>>(notificationsEnt);
            
            return notificationsVM;
        }

        public async Task<UserLoggedIn?> GetUserInfoAsync(int userId)
        {
            UserLoggedIn? userInfo = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserLoggedIn { UserId = u.Id, Username = u.Username, Email = u.Email, RolesId = u.Roles.Select(r => r.Id).ToList() })
                .FirstOrDefaultAsync();

            return userInfo;
        }
    }
}
