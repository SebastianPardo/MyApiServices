using AuthServices.Business.Interfaces;
using Famnances.Core.Entities;
using Famnances.Core.Security;
using Famnances.Core.Security.Services.Interfaces;

namespace AuthServices.Security
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        ITokenHandler _tokenHandler;

        public JwtMiddleware(RequestDelegate next, ITokenHandler tokenHandler)
        {
            _next = next;
            _tokenHandler = tokenHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Path.Value.Contains("Authenticate"))
                    await _next(context);
                else
                {
                    var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                    TokenContent? tokenContent = _tokenHandler.ValidateToken(token);
                    if (tokenContent != null && 
                        (context.Request.Host.Port == 7246 
                        || context.Request.Host.Port == 7239 
                        || context.Request.Host.Port == 7238 
                        || context.Request.Host.Value.Contains("famnances")
                        || context.Request.Host.Value.Contains("famnancesservices")
                        || context.Request.Host.Value.Contains("sp-authservices")))
                    {
                        var accountManager = context.RequestServices.GetRequiredService<IAccountService>();
                        var account = accountManager.GetById(tokenContent.UserId);
                        context.Items[Constants.ACCOUNT_ID] = account.Id;
                        await _next(context);
                    }
                    else
                        context.Abort();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
