using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SSOAttribute : Attribute, IAuthorizationFilter
    {
        private AuthorizationFilterContext? _context;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                _context = context;
                var ssoConnectionService = _context.HttpContext.RequestServices.GetService<SsoConnectionService>()!;
                var appSettings = _context.HttpContext.RequestServices.GetService<AppSettings>()!;
                var httpType = _context.HttpContext.Request.Method.ToUpperInvariant();
                var accessToken = string.Empty;

                if (httpType == "POST" || httpType == "PUT" || httpType == "PATCH")
                    if (context.HttpContext.Request.Headers.TryGetValue("access-token", out var token))
                        accessToken = token;

                if (httpType == "GET")
                    accessToken = _context.HttpContext.Request.Query["accessToken"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var userId = ssoConnectionService.ValidateToken(accessToken).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        appSettings.UserId = new ObjectId(userId);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                if (_context is not null)
                    _context.HttpContext.Response.StatusCode = new UnauthorizedResult().StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
