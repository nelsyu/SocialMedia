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

        public async Task<List<string>> ValidateRegisterAsync(UserViewModel userVM)
        {
            string IsEmailValidate = await IsValidEmailAsync(userVM.Email);
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

        public async Task<List<string>> ValidatePostAsync(PostViewModel postVM)
        {
            bool isTopicIdInvalid = !await _dbContext.Topics.AnyAsync(t => t.Id == postVM.TopicId);
            bool isTitleInvalid = string.IsNullOrEmpty(postVM.Title);
            bool isContentInvalid = string.IsNullOrEmpty(postVM.Content);

            List<string> result = new();

            if (isTopicIdInvalid)
            {
                result.Add("Id");
                result.Add("Id is invalid.");
            }
            else if (isTitleInvalid)
            {
                result.Add("Title");
                result.Add("Title is invalid.");
            }
            else if (isContentInvalid)
            {
                result.Add("Content");
                result.Add("Content is invalid");
            }
            else
            {
                result.Add("");
            }

            return result;
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
