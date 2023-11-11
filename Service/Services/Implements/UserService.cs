using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using Service.Services.Interfaces;
using Service.ViewModels;
using Service.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;
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

        public List<string> ValidateRegister(UserViewModel userVM)
        {
            string IsEmailValidate = IsValidEmail(userVM.Email);
            bool IsUsernameInvalid = _dbContext.Users.Any(u => u.Username == userVM.Username) || userVM.Username == null;
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
            else if(IsPasswordInvalid)
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

        public void Register(UserViewModel userVM)
        {
            User userEnt = _mapper.Map<User>(userVM);
            _dbContext.Users.Add(userEnt);
            _dbContext.SaveChanges();
        }

        public List<string> ValidateLogin(UserViewModel userVM, object captchaCode)
        {
            bool IsEmailInvalid = !_dbContext.Users.Any(u => u.Email == userVM.Email);
            bool IsPasswordInvalid = !_dbContext.Users.Any(u => u.Password == userVM.Password);
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
            else if(IsConfirmCaptchaInvalid)
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

        public void LoginSuccessful(string UserVMEmail)
        {
            User? userEnt = _dbContext.Users.Where(u => u.Email == UserVMEmail).FirstOrDefault();
            if (userEnt != null)
            {
                UserViewModel userVM = _mapper.Map<UserViewModel>(userEnt);
                _userLoggedIn.Set(userVM);
            }
        }

        public bool IsLogin()
        {
            bool IsLogin = !string.IsNullOrEmpty(_userLoggedIn.Username);
                return IsLogin;
        }

        public void Logout()
        {
            _userLoggedIn.Dispose();
        }

        public void DeleteAccount(UserViewModel userVM)
        {
            User? userEnt = _dbContext.Users.Where(u => u.Username == _userLoggedIn.Username).FirstOrDefault();
            
            if(userEnt != null)
            {
                _dbContext.Users.Remove(userEnt);
                _dbContext.SaveChanges();
                _userLoggedIn.Dispose();
            }
        }

        public void SaveQRCodeOTP(string QRCodeOTPSK)
        {
            User? userEnt = _dbContext.Users.FirstOrDefault(u => u.Username == _userLoggedIn.Username);
            if (userEnt != null)
            {
                userEnt.Totp = QRCodeOTPSK;
                _dbContext.SaveChanges();
            }
        }

        public string ?IsQRCodeOTPSecretKey(string UserVMEmail)
        {
            string? QRCodeOTPSecretKey = _dbContext.Users
                .Where(u => u.Email == UserVMEmail)
                .Select(u => u.Totp)
                .FirstOrDefault();

            return QRCodeOTPSecretKey;
        }

        public List<string> VerifyQRCodeOTP(string userVMEmail, string confirmQRCodeOTP)
        {
            string? qRCodeOTPSecretKey = _dbContext.Users
                .Where(u => u.Email == userVMEmail)
                .Select(u => u.Totp)
                .FirstOrDefault();

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

        public byte[] GenerateCaptchaImage(out string captchaCode)
        {
            //TODO:安装 SkiaSharp.NativeAssets.Linux.NoDependencies 包
            var validateCodeProvider = new SkiaSharpValidateCodeHelper
            {
                SetWith = 100,
                SetHeight = 40,
                SetFontSize = 20
            };

            byte[] img = validateCodeProvider.GetVerifyCodeImage();
            captchaCode = validateCodeProvider.SetVerifyCodeText;

            return img;
        }

        public byte[] GenerateOTPQRCode(out string secretKey)
        {
            secretKey = Base32Encoding.ToString(OtpNet.KeyGeneration.GenerateRandomKey());

            // 顯示在 APP 中的發行者名稱
            string issuer = "SocialMedia";

            // 顯示在 APP 中的標題
            string label = _userLoggedIn.Username ?? "";
            string keyUri = new OtpUri(OtpType.Totp, secretKey, label, issuer).ToString();

            byte[] image = PngByteQRCodeHelper.GetQRCode(keyUri, QRCodeGenerator.ECCLevel.Q, 10);

            return image;
        }

        #region private methods
        private string IsValidEmail(string email)
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
