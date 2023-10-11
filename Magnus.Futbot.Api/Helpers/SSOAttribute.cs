using Magnus.Futbot.Api.Services.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Magnus.Futbot.Api.Helpers
{
    public class SSOVerificationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context?.HttpContext;
            var accessToken = httpContext?.Request.Headers.Authorization;
            var userId = await httpContext?.RequestServices?.GetService<SsoConnectionService>()?.ValidateToken(accessToken);

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            httpContext.User.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim("UserId", userId)
            }));
        }
    }
}
