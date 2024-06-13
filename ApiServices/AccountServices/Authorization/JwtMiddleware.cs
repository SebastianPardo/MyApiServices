using Famnances.AuthMiddleware;
using AccountServices.Business.Interfaces;
using Microsoft.Extensions.Options;
using Famnances.AuthMiddleware.Models;

namespace AuthenticationServices.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        JwtTokenHandler JwtTokenHandler;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            JwtTokenHandler = new JwtTokenHandler(appSettings.Value);
        }

        public async Task Invoke(HttpContext context, IAccountManager accountManager)
        {
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string? userId = JwtTokenHandler.ValidateToken(token);
                if (userId != null)
                {
                    context.Items["User"] = accountManager.getByUserNameOrEmail(userId);
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }        
    }
}
