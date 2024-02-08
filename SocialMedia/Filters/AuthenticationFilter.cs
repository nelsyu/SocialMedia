using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.Services.Interfaces;

namespace SocialMedia.Filters
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private readonly HttpContext _httpContext;

        public AuthenticationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_httpContext!.User.Identity!.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }
    }
}
