using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Famnances.DataCore.Entities;
using Famnances.AuthMiddleware.Models;
using AccountServices.Business.Interfaces;

namespace AccountServices.Authorization
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorization : ActionFilterAttribute, IAuthorizationFilter
    {
        private const string ID = "ACCOUNT_ID";
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            var allowAnonymous = filterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            User user = (User)filterContext.HttpContext.Items["User"];
            if (user == null)
                filterContext.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            try
            {
                var accountId = filterContext.HttpContext.Session.GetString(ID);

                var path = filterContext.HttpContext.Request.Path.Value ?? string.Empty;
                var accountManager = filterContext.HttpContext.RequestServices.GetRequiredService<IAccountManager>();

                Account? account = accountManager.GetById(Guid.Parse(accountId));

                if (account == null)
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

