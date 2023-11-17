﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.Services.Interfaces;

namespace SocialMedia.Filters
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private readonly IUserService _userService;

        public AuthenticationFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!await _userService.IsLoginAsync())
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }
    }
}
