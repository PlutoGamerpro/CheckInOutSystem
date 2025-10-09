using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TimeRegistration.Services;
using TimeRegistration.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TimeRegistration.Filters
{
	// Reusable administrator authorization attribute
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAuthorizeAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var sp = context.HttpContext.RequestServices;
			var authService = sp.GetRequiredService<IAuthenticationService>();

			var authResult = await authService.AuthenticateAsync(context.HttpContext, JwtBearerDefaults.AuthenticationScheme);
			if (!authResult.Succeeded || authResult.Principal is null)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			context.HttpContext.User = authResult.Principal;

			if (!authResult.Principal.IsInRole("Admin") && !authResult.Principal.IsInRole("Manager"))
			{
				context.Result = new ForbidResult();
				return;
			}

			await next();
		}
	}
}
