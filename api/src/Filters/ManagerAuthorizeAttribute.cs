using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TimeRegistration.Services;

namespace TimeRegistration.Filters // file looks like adminauthorize
{
    // Reusable manager authorization attribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ManagerAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

            var authResult = await authService.AuthenticateAsync(context.HttpContext, JwtBearerDefaults.AuthenticationScheme);
            if (!authResult.Succeeded || authResult.Principal is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.User = authResult.Principal;

            if (!authResult.Principal.IsInRole("Manager") && !authResult.Principal.IsInRole("Admin"))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
