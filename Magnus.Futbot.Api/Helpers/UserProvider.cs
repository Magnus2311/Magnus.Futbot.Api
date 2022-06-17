using Magnus.Futbot.Api.Services.Connections;
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
            return httpContext?.RequestServices?.GetService<SsoConnectionService>()?.ValidateToken(sv.FirstOrDefault() ?? string.Empty).GetAwaiter().GetResult();
        }
    }
}