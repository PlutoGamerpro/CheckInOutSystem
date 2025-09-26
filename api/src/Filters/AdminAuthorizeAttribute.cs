using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TimeRegistration.Services;

namespace TimeRegistration.Filters
{
	// Reusable administrator authorization attribute
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAuthorizeAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var headers = context.HttpContext.Request.Headers;
			// Accept both admin and manager tokens (mesmo validador)
			var token = headers["X-Admin-Token"].FirstOrDefault()
						?? headers["X-Manager-Token"].FirstOrDefault()
						?? "";
			var auth = context.HttpContext.RequestServices.GetService<IAdminAuthService>();
			if (auth == null || !auth.Validate(token))
			{
				context.Result = new UnauthorizedResult();
				return;
			}
			// FUTURO: extrair role do token e checar permissões específicas.
			await next();
		}
	}
}
