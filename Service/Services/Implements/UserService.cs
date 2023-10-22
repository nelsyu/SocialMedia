using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Services.Interfaces;
using Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly SocialMediaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(SocialMediaContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> ValidateRegister(string email, string Username)
        {
            var IsEmailUnique = !_dbContext.Users.Any(u => u.Email == email);
            var IsUsernameUnique = !_dbContext.Users.Any(u => u.Username == Username);
            
            List<string> result = new();

            if (!IsEmailUnique)
            {
                result.Add("Email");
                result.Add("Email is already registered.");
            }
            else if (!IsUsernameUnique)
            {
                result.Add("Username");
                result.Add("Username is already registered.");
            }
            else
                result.Add("");
            return result;
        }

        public void Register(UserViewModel userVM)
        {
            // 使用 AutoMapper 將 UserViewModel 轉換為 User
            var userMap = _mapper.Map<User>(userVM);

            // 將 User 寫入資料庫
            _dbContext.Users.Add(userMap);
            _dbContext.SaveChanges();
        }

        public List<string> ValidateLogin(string email, string password)
        {
            var IsEmailValid = _dbContext.Users.Any(u => u.Email == email);
            var IsPasswordValid = _dbContext.Users.Any(u => u.Password == password);

            List<string> result = new();

            if (!IsEmailValid)
            {
                result.Add("Email");
                result.Add("Email is invalid.");
            }
            else if (!IsPasswordValid)
            {
                result.Add("Password");
                result.Add("Password is invalid.");
            }
            else
            {
                result.Add("");

                var User = _dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
                if (User != null)
                {
                    var UserVM = _mapper.Map<UserViewModel>(User);
                    _httpContextAccessor.HttpContext?.Session.SetString("Username", UserVM.Username);
                }
            }
            return result;
        }

        public bool IsLogin()
        {
            bool IsLogin = !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("Username"));
                return IsLogin;
        }

        public void Logout()
        {
            // 清除 Session 中的 Username
            _httpContextAccessor.HttpContext?.Session.Remove("Username");
        }
    }
}
