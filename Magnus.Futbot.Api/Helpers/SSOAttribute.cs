using Magnus.Futbot.Api.Services.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace Magnus.Futbot.Api.Helpers
{
    public class SSOVerificationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            StringValues sv = new StringValues();
            var httpContext = context?.HttpContext;
            httpContext?.Request.Query.TryGetValue("access_token", out sv);
            var userId = await httpContext?.RequestServices?.GetService<SsoConnectionService>()?.ValidateToken(sv.FirstOrDefault() ?? string.Empty);

            if (userId == null)
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
