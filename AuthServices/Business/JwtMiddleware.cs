using AccountServices.Business.Interfaces;
using Microsoft.Extensions.Options;
using Famnances.AuthMiddleware.Entities;
using Famnances.AuthMiddleware;
using Famnances.AuthMiddleware.Interfaces;

namespace AccountServices.Business
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
                    if (tokenContent != null)
                    {
                        var accountManager = context.RequestServices.GetRequiredService<IAccountService>();
                        var account = accountManager.GetById(tokenContent.UserId);
                        context.Items[Constants.USER] = account.Id;
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
