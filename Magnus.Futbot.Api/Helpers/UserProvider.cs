using System.Security.Claims;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace Magnus.Futbot.Api.Helpers
{
    public class UserProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            StringValues sv = new StringValues();
            var httpContext = connection?.GetHttpContext();
            httpContext?.Request.Query.TryGetValue("access_token", out sv);
            var appSettings = httpContext?.RequestServices.GetService<AppSettings>()!;
            var userId = httpContext?.RequestServices?.GetService<SsoConnectionService>()?.ValidateToken(sv.FirstOrDefault() ?? string.Empty).GetAwaiter().GetResult();

            connection!.GetHttpContext()!.User = new ClaimsPrincipal(new CustomPrincipal(userId ?? ""));
            return userId;
        }
    }
}