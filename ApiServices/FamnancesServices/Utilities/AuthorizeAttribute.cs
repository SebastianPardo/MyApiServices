using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Famnances.AuthMiddleware;
using Famnances.AuthMiddleware.Models;
using Microsoft.Extensions.Options;

namespace FamnancesServices.Utilities
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private const string TOKEN = "TOKEN";
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            var allowAnonymous = filterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            string? token = (string?)filterContext.HttpContext.Items[TOKEN];
            if (token == null)
                filterContext.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            try
            {
                var path = filterContext.HttpContext.Request.Path.Value ?? string.Empty;
                var accountManager = filterContext.HttpContext.RequestServices.GetRequiredService<IAccountManager>();
                var appSettings = filterContext.HttpContext.RequestServices.GetRequiredService<IOptions<AppSettings>>();
                var tokenHandler = new JwtTokenHandler(appSettings.Value);

                TokenContent? tokenContent = tokenHandler.ValidateToken(token);
                Account? account = tokenContent != null ? accountManager.GetById(tokenContent.UserId) : null;
                filterContext.HttpContext.Items["AccountId"] = tokenContent != null ? tokenContent.UserId : null;

                if (account == null || !(account.Email == tokenContent.Email || account.UserName == tokenContent.User))
                {
                    filterContext.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch
            {
                filterContext.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

        }
    }
}

