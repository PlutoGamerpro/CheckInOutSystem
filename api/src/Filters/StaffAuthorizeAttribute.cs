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

	// old name AdminAuthorizeAttribute
	public class StaffAuthorizeAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var headers = context.HttpContext.Request.Headers;
			var token = headers["X-Admin-Token"].FirstOrDefault()
						?? headers["X-Manager-Token"].FirstOrDefault()
						?? "";

			if (string.IsNullOrWhiteSpace(token))
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var sp = context.HttpContext.RequestServices;
			var adminAuth = sp.GetService<IAdminAuthService>();

			var ok = adminAuth != null && adminAuth.Validate(token);

			if (!ok)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			await next();
		}
	}
}
