using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAngular.Extensions
{
	public static class AuthenticationMiddleware
	{
		public static IApplicationBuilder UseSpaAuthentication(this IApplicationBuilder app)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			return app.UseMiddleware<SpaAuthMiddleware>();
		}
	}

	internal class SpaAuthMiddleware
	{
		private readonly RequestDelegate _next;

		public SpaAuthMiddleware(RequestDelegate next)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
		{
			if (!context.User.Identity.IsAuthenticated)
			{
				await context.ChallengeAsync().ConfigureAwait(false);
			}
			else
			{
				await _next(context).ConfigureAwait(false);
			}
		}
	}
}
