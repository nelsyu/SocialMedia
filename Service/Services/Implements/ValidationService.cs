using Data.Entities;
using Lazy.Captcha.Core;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using System.Text.RegularExpressions;

namespace Service.Services.Implements
{
    public class ValidationService : IValidationService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly ICaptcha _captcha;

        public ValidationService(SocialMediaContext dbContext, ICaptcha captcha)
        {
            _dbContext = dbContext;
            _captcha = captcha;
        }

        public async Task<List<(string key, string errorMessage)>> ValidateRegisterAsync(UserViewModel userVM)
        {
            string IsEmailValidate = await IsValidEmailAsync(userVM.Email);
            bool IsUsernameInvalid = await _dbContext.Users.AnyAsync(u => u.Username == userVM.Username) || userVM.Username == null;
            bool IsPasswordInvalid = string.IsNullOrEmpty(userVM.Password);
            bool IsConfirmPasswordInvalid = string.IsNullOrEmpty(userVM.ConfirmPassword) || userVM.Password != userVM.ConfirmPassword;
            bool IsConfirmCaptchaInvalid = !_captcha.Validate(userVM.UId, userVM.CaptchaCode);
            List<(string key, string errorMessage)> errors = new();

            if (IsEmailValidate != "true")
            {
                errors.Add(("Email", IsEmailValidate));
            }

            if (IsUsernameInvalid)
            {
                errors.Add(("Username", "Username is invalid."));
            }

            if (IsPasswordInvalid)
            {
                errors.Add(("Password", "Password is invalid."));
            }

            if (IsConfirmPasswordInvalid)
            {
                errors.Add(("ConfirmPassword", "ConfirmPassword is invalid."));
            }

            if (IsConfirmCaptchaInvalid)
            {
                errors.Add(("CaptchaCode", "Captcha Code is invalid"));
            }

            return errors;
        }

        public async Task<List<(string key, string errorMessage)>> ValidateLoginAsync(UserViewModel userVM)
        {
            bool IsEmailInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email);
            bool IsPasswordInvalid = !await _dbContext.Users.AnyAsync(u => u.Email == userVM.Email && u.Password == userVM.Password);
            bool IsConfirmCaptchaInvalid = !_captcha.Validate(userVM.UId, userVM.CaptchaCode);
            List<(string key, string errorMessage)> errors = new();

            if (IsEmailInvalid)
            {
                errors.Add(("Email", "Email is invalid."));
            }
            
            if (IsPasswordInvalid)
            {
                errors.Add(("Password", "Password is invalid."));
            }
            
            if (IsConfirmCaptchaInvalid)
            {
                errors.Add(("CaptchaCode", "Captcha Code is invalid"));
            }

            return errors;
        }

        public async Task<List<(string key, string errorMessage)>> ValidatePostAsync(PostViewModel postVM)
        {
            bool isTopicIdInvalid = !await _dbContext.Topics.AnyAsync(t => t.Id == postVM.TopicId);
            bool isTitleInvalid = string.IsNullOrEmpty(postVM.Title);
            bool isContentInvalid = string.IsNullOrEmpty(postVM.Content);
            List<(string key, string errorMessage)> errors = new();

            if (isTopicIdInvalid)
            {
                errors.Add(("Id", "Id is invalid."));
            }
            
            if (isTitleInvalid)
            {
                errors.Add(("Title", "Title is invalid."));
            }
            
            if (isContentInvalid)
            {
                errors.Add(("Content", "Content is invalid"));
            }

            return errors;
        }

        #region private methods
        private async Task<string> IsValidEmailAsync(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrWhiteSpace(email))
            {
                return "電子郵件地址不能為空。";
            }

            if (await _dbContext.Users.AnyAsync(u => u.Email == email))
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
