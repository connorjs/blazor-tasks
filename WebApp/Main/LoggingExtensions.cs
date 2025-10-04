using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class LoggingExtensions
{
	internal static T AddMyLogging<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		// appsettings.* defines logging configuration (12-Factor)
		builder
			.Logging.ClearProviders()
			.AddConsole()
			.Configure(static o =>
			{
				o.ActivityTrackingOptions =
					ActivityTrackingOptions.SpanId
					| ActivityTrackingOptions.TraceId
					| ActivityTrackingOptions.ParentId
					| ActivityTrackingOptions.Tags
					| ActivityTrackingOptions.Baggage;
			});
		builder.Services.AddHttpLogging(static o =>
		{
			o.LoggingFields =
				HttpLoggingFields.Duration
				| HttpLoggingFields.RequestPath
				| HttpLoggingFields.RequestProtocol
				| HttpLoggingFields.RequestMethod
				| HttpLoggingFields.ResponseStatusCode;
			o.RequestHeaders.Add("User-Agent");
			o.ResponseHeaders.Add("Content-Type");
			o.CombineLogs = true;
		});
		return builder;
	}

	internal static WebApplication UseMyLogging(this WebApplication app)
	{
		app.UseHttpLogging();
		return app;
	}
}
