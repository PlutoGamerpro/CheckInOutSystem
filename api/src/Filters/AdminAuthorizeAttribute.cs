using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TimeRegistration.Services;

namespace TimeRegistration.Filters
{
	// Atributo de autorização admin reutilizável
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAuthorizeAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var token = context.HttpContext.Request.Headers["X-Admin-Token"].FirstOrDefault() ?? "";
			var auth = context.HttpContext.RequestServices.GetService<IAdminAuthService>();
			if (auth == null || !auth.Validate(token))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			await next();
		}
	}
}
