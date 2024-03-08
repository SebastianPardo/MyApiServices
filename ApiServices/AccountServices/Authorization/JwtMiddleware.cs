using OhMyMoney.AuthMiddleware;
using AccountServices.Business;
using OhMyMoney.DataCore.Data;

namespace AuthenticationServices.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate RequestDelegate;
        JwtTokenHandler JwtTokenHandler;
        public JwtMiddleware(RequestDelegate requestDelegate, JwtTokenHandler jwtTokenHandler)
        {
            JwtTokenHandler = jwtTokenHandler;
            RequestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            var dbcontext = context.RequestServices.GetRequiredService<DatabaseContext>();
            var userManager = new AccountManager(dbcontext);
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string? userId = JwtTokenHandler.ValidateToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = userManager.getByUserNameOrEmail(userId);
            }

            await RequestDelegate(context);
        }        
    }
}
