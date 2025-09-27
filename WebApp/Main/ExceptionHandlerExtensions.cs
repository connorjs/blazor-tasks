using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class ExceptionHandlerExtensions
{
	internal static WebApplication UseMyExceptionHandler(this WebApplication app)
	{
		app.UseExceptionHandler(errorApp =>
			errorApp.Run(async ctx =>
			{
				var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogError(
					ctx.Features.Get<IExceptionHandlerFeature>()?.Error,
					"Unhandled exception"
				);

				ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
				ctx.Response.ContentType = "application/problem+json";
				await Results.Problem(statusCode: 500, title: "Unexpected error").ExecuteAsync(ctx);
			})
		);
		return app;
	}
}
