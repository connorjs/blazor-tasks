using FastEndpoints;
using Microsoft.AspNetCore.Builder;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class ExceptionHandlerExtensions
{
	internal static WebApplication UseMyExceptionHandler(this WebApplication app)
	{
		// https://fast-endpoints.com/docs/exception-handler
		app.UseDefaultExceptionHandler(logStructuredException: true, useGenericReason: true);
		return app;
	}
}
