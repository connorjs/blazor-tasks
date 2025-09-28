using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static partial class ExceptionHandlerExtensions
{
	internal static WebApplication UseMyExceptionHandler(this WebApplication app)
	{
		app.UseExceptionHandler(errorApp =>
			errorApp.Run(async ctx =>
			{
				var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogUnexpectedError(ctx.Features.Get<IExceptionHandlerFeature>()?.Error);

				await Results
					.Problem(
						statusCode: StatusCodes.Status500InternalServerError,
						title: "Unexpected error"
					)
					.ExecuteAsync(ctx);
			})
		);
		return app;
	}

	[LoggerMessage(EventId = 5000, Level = LogLevel.Error, Message = "Unhandled exception")]
	public static partial void LogUnexpectedError(this ILogger logger, Exception? exception);
}
